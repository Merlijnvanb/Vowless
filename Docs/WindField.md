# Global Reactive Force Field ("WindField")

A cosmetic, world-scale force field that makes the environment feel alive and react to the
players. Swings inject disturbances; everything in the world (cloth, banners, grass, particles,
hair, dust) samples the field and responds.

**These are research notes, not a spec. Nothing here is set in stone.**

Two different kinds of content live in this document, and they deserve different levels of trust:

- **Durable** — the math (§4), the lessons (§9), the references (§11). These are facts about how
  curl noise, potential flow and Unity's noise API actually behave. They hold regardless of what
  gets built.
- **Provisional** — every architectural decision (§1, §2, §5, §6). Each one rests on an
  assumption, and each is written with its *reasoning attached* so you can see exactly which
  assumption it depends on and re-open it the moment that assumption stops holding.

This project has already reversed several "obvious" decisions mid-flight — the follow-player
clipmap, the sink direction, the layer-blending operator. Expect more. The value here is the
accumulated understanding, not the conclusions.

---

## 1. Where the thinking landed (and what would move it)

Each item names the assumption it rests on. Kill the assumption, and the decision is back on the table.

1. **View-layer only. NOT the Quantum simulation.** This is purely cosmetic — it never affects
   the fight. Therefore: floats, no determinism, no rollback, and it must never enter the
   Quantum frame buffer (a 3D grid there would be copied and checksummed on every rollback).
2. **The stage is a circular loop.** In sim logic the x-axis wraps at a fixed circumference; in
   view it's a loop you walk around the inside of. The domain is therefore **bounded and
   periodic** — which means **no follow-player window, no clipmap, no toroidal streaming
   machinery**. That whole family of open-world techniques solves a problem we don't have.
3. **2.5D.** Players move only in X and Y. The scenery behind the walking plane is deep (Z) and
   much taller (Y) than the players. **Size the reactive volume to the scenery, not to the
   players.** It contains both — impulses are injected right where the players are — but its
   extents are set by the tall, deep scenery that has to react, not by the thin band the players
   occupy. Inject small, sample big; propagation bridges the two.
4. **There is a focal point far in the back.** It's the permanent visual center of the
   composition; environment elements curve toward and around it.

---

## 2. Architecture: two layers, summed

Every consumer reads one function:

```
SampleForce(worldPos) = Ambient(worldPos) + Reactive(worldPos)
```

### Layer 1 — Ambient (analytic, zero storage)

A pure function of position and time. No grid, no state, infinite extent.

- **Spiral sink** — a potential-flow *vortex + sink* anchored at the focal point. This is the
  "everything curves toward the center" structure.
- **Curl noise** — organic, swirling turbulence layered on top.

Because it's a pure function, **the CPU and GPU compute it identically with zero synchronization.**
This is what makes dual CPU/GPU access nearly free.

### Layer 2 — Reactive (a stored grid)

The only thing that lives in memory.

- A **fixed Cartesian grid**: periodic in `u` (the loop axis), tall in `v`, deep in `w`.
- Impulses (sword swings) are injected in a thin band near the players.
- **Semi-Lagrangian advection + decay.** No pressure projection.
- Advected along `grid + ambient`, so disturbances propagate **and** curve toward the focal point.
- Vorticity confinement comes later, to restore swirl that advection smears away.

---

## 3. Why this works — the cheats

- **Divergence-free for free.** Both curl noise and potential flow are divergence-free *by
  construction*. Incompressibility is what makes a field read as "fluid," and it's normally the
  most expensive part of a fluid sim (a globally-coupled Poisson/pressure solve, every frame).
  We get it from local math instead, and **skip the pressure solve entirely.**
- **The spiral sink doubles as the advection guide field.** The same field that gives the ambient
  its center-organized look also steers the reactive grid's disturbances along curved paths
  toward the focal point. Curved propagation without a curved grid.
- **Polar/cylindrical grids were considered and rejected.** They have a nasty center-axis
  singularity, and they allocate resolution *backwards* — fine detail at the far, unimportant
  center; coarse cells where the players actually are. We get the polar *aesthetic* from the
  analytic layer while the stored grid stays a plain uniform box.

---

## 4. The math

### 4.1 Curl noise

Curl noise is not a noise type — it's an *operation on* noise. Intuition: take a random bumpy
landscape and, at every point, make the wind blow **sideways along the slope** (the uphill
direction, rotated 90°). The flow circles every hill instead of draining into it, so nothing
ever piles up. That's incompressibility.

Two implementations exist in `AmbientForce.cs`, switchable via `CurlMethod`.

**Method A — vector potential + finite-difference curl** (3 noise fields, 3 offsets)

```
P(p)  = ( snoise(p + o0), snoise(p + o1), snoise(p + o2) )     // the vector potential ψ

dNdx  = ( P(p + εx) − P(p − εx) ) / (2ε)                        // ∂P/∂x, a float3
dNdy  = ( P(p + εy) − P(p − εy) ) / (2ε)
dNdz  = ( P(p + εz) − P(p − εz) ) / (2ε)

curl  = ( dNdy.z − dNdz.y,  dNdz.x − dNdx.z,  dNdx.y − dNdy.x )
```

More expensive (18 `snoise` calls) but exposes three independent components — the **artistic**
option (per-component weighting gives anisotropic flow; animating the offsets makes it churn).

**Method B — cross product of two gradients** (2 noise fields, 1 offset)

```
noise.snoise(p,          out gradF);    // Unity.Mathematics gives the ANALYTIC gradient
noise.snoise(p + 1008.5, out gradG);
curl = normalize( cross(gradF, gradG) );
```

Two calls, exact gradients, no epsilon. The **fast, clean default.**

> `∇f × ∇g = ∇ × (f ∇g)` — so Method B is *still* the curl of a potential, just an implicit
> one (`f∇g`). Both methods are divergence-free for the same underlying reason. Their potentials
> differ in structure (three free noise fields vs. the constrained `f∇g`), which is exactly why
> the two produce different-looking flow.

**Method B must be normalized.** `|∇f × ∇g| = |∇f|·|∇g|·sin θ` — a *product* of two gradient
magnitudes, so it has enormous dynamic range and collapses to ~zero wherever the gradients
happen to align. Its raw magnitude is a numerical artifact, not a wind speed. Method A's
magnitude *is* usable — don't normalize that one.

### 4.2 Spiral sink (potential flow)

The classic **"spiral sink"** = a free vortex + a sink. Streamlines are logarithmic spirals that
circulate around the focal point *while converging on it*. Think of water draining down a plughole.

```
axis         = SpiralAxis.normalized                        // MUST be unit length

offset       = point − FocalPoint.position
radialOffset = offset − dot(offset, axis) * axis
radius       = max(|radialOffset|, MinRadius)               // ⟂ to the axis  → for the VORTEX
distance     = max(|offset|,       MinRadius)               // full 3D        → for the SINK

sinkVel      = (−SinkStrength / distance) * (offset / distance)          // toward the POINT
vortexVel    = ( VortexStrength / radius ) * cross(axis, radialOffset/radius)  // around the AXIS
```

> ⚠️ **The trap.** A *vortex* is intrinsically about an **axis** (rotation in 3D is always around
> a line). A *sink* toward the focal point is intrinsically about a **point**. These need
> different geometry. In 2D they look identical — "radial" means the same thing — so the
> distinction is invisible in the textbook formulas and only appears in 3D. Using the
> perpendicular-to-axis `radialDir` for the sink makes flow drain to the *axis line* and orbit
> forever, never reaching the point. This was a real bug we hit.

The `1/r` falloff is a feature: the spiral dominates near the center and fades far out, where
curl noise takes over. The two layers hand off automatically. Softening the falloff (e.g.
`1/sqrt(r)`) extends the spiral's reach — not physically pure, but this is cosmetic, so tune it.

### 4.3 Combining the layers

```
Ambient = Spiral(p) * SpiralScalar + Curl(p) * CurlScalar        // ADD. Never Lerp.
```

**Never `Lerp`.** A lerp is a *crossfade* — it trades one layer for the other, so you can never
have a strong spiral *and* strong turbulence at once, which is the entire "organized but alive"
goal. The two scalars already give independent control. Add them; two winds superimpose.

**Only the curl scrolls.** Animate by offsetting the *noise* coordinate with
`ScrollDirection * ScrollSpeed * Time.time`. Do **not** scroll the spiral's input — the spiral is
anchored to the focal point, and scrolling its input silently drags the focal point across the
world. (Sliding noise reads as "wind carrying turbulence along," which suits wind. 4D noise
scrolled through the 4th dimension would boil in place instead — but the analytic-gradient
overload is 3D-only, so Method B can't do that.)

---

## 5. Coordinate system (needed from Rung 4 onward)

Store the field in the stage's **logical loop-space** `(u, v, w)`:

- `u` — position around the loop. **Periodic**, with the same wrap length the gameplay sim uses.
- `v` — height. Bounded, but **tall** — it must span the background scenery, not just the players.
- `w` — depth into the scene. Bounded.

The field is a flat, rectilinear, periodic-in-`u` box. It knows nothing about the world being
visually curved into a ring; the **view layer** owns that mapping. Ignore the metric distortion
from the curvature — invisible for cosmetic wind.

Because `u` wraps, every `u`-axis operation (advection backtrace, neighbour reads, injection
splat, trilinear sampling) indexes with **modulo**, not clamp. A gust can travel all the way
around the stage — which is physically correct on a circular arena.

**Seam warning:** curl noise will tear at the wrap point unless it tiles with period =
circumference. Fix by mapping `u → θ` and sampling noise on a circle, `(sin θ, cos θ)`; or use
`noise.pnoise(float3, float3 rep)` (periodic Perlin) or 4D noise.

---

## 6. Dual CPU/GPU access

- **Ambient:** a pure function. CPU evaluates it in a Burst job, GPU evaluates the same function
  in a shader, same params → automatically identical. **Nothing to sync.**
- **Reactive grid:** **CPU-authoritative.** A `NativeArray<float4>` (use `float4`, not `float3`,
  for SIMD alignment; index `(z*H + y)*W + x`), uploaded into a `Texture3D` once per frame. GPU
  consumers sample the texture (hardware trilinear filtering); CPU consumers read the array
  directly with zero latency.
- **One writer, ever.** CPU simulates; GPU is a read-only consumer. Never let both write.
- Never use synchronous `GetData` (it stalls the pipe). If a GPU→CPU read is ever needed, use
  `AsyncGPUReadback.RequestIntoNativeArray` and budget 2–3 frames of latency.
- Caching the analytic ambient into a `Texture3D` is **cosmetically free** — exact particle
  trajectories drift versus the analytic field, but the *character* of the motion is preserved.

---

## 7. Build ladder

| Rung | Work | Layer | Status |
|-----:|------|-------|--------|
| 0 | Debug visualisation (gizmo plane, strand, particle) | instrument | ✅ done |
| 1 | Curl noise (both methods) | ambient | ✅ done |
| 2 | Spiral sink | ambient | ✅ done |
| 3 | Hook `ClothSim` — replace its uniform `WindVector` with `SampleForce(point.Pos)` | consumer | ⬜ **next** |
| 4 | Reactive grid: storage, world→(u,v,w) mapping, `u`-wrap, injection, trilinear sampling | reactive | ⬜ |
| 5 | Semi-Lagrangian advection + frame-rate-independent decay (**propagation**) | reactive | ⬜ |
| 6 | Vorticity confinement; seamless noise tiling; Burst; `Texture3D` upload; more consumers | both | ⬜ |

Ambient was deliberately built first: it's cheap, grid-free, permanently useful, and it forces
you to build the three things the reactive layer also needs — the **visualiser**, the
**`SampleForce` seam**, and the **coordinate mapping**.

---

## 8. Open TODOs in `AmbientForce.cs`

- **`curl *= 300f` is a magic number.** The finite differences compute only the numerator
  (`P(+ε) − P(−ε) ≈ 2ε·P'`), so the result scales *with* `Epsilon` and 300 secretly compensates
  for `ε = 0.001`. **Divide by `2 * Epsilon`** to get a true derivative, then delete the 300 and
  let `CurlScalar` own the magnitude. Otherwise `Epsilon` silently controls strength as well as
  accuracy.
- The scroll offset is applied *before* the `NoiseScale` multiply, so effective scroll speed
  scales with `NoiseScale`. Decide whether that's intended.
- Dead `using` directives.

---

## 9. Lessons learned (hard-won; don't rediscover these)

- **Potential offsets are seeds, not style knobs.** Only the *pairwise separation* `|oᵢ − oⱼ|`
  matters — direction is arbitrary, and distance from the sample point is irrelevant (it cancels).
  Keep them hundreds of units apart. If offsets are equal (or zero), the components correlate and
  the curl collapses to zero. They become *artistically* interesting only when **animated**.
- **The curl of jaggy noise is garbage.** Curl differentiates the potential, which amplifies any
  roughness. Use smooth noise. `ε` too small → floating-point hash; too big → swirls smeared away.
- **Switching curl methods changes magnitude a lot** (normalized vs. raw). Expect to re-tune
  `CurlScalar`.
- **The debug strand should step along a *normalized* direction.** Stepping by raw magnitude makes
  spacing track field strength, which looks violently jagged with Method B.
- **The strand tracer *is* the advection integrator.** Stepping a point along the velocity field is
  exactly what Rung 5 does. Reuse the idea.
- **Reactivity weakens with distance from the swing.** That's physically right. If you want the
  tall/deep scenery to react more, inject impulses with a directional bias (a `+w` or `+v` push)
  rather than cranking propagation.
- Inject **swept, not point** — a slash is an arc; splat along the blade's path. And inject **curl,
  not just push** — a blade's wake is a vortex, not a shove. This is the single biggest feel win.
- Decay must be frame-rate independent: `exp(-k * dt)`, never a flat multiply.

---

## 10. Filed for later

**SDF surface-flow.** Taking the curl of `(SDF gradient × noise)` produces flow that runs *parallel
to a surface*. This is the tool for making wind hug the environment geometry — walls, banners, the
central focal structure that elements "curve along." Not needed for the prototype; a strong lever
afterwards.

---

## 11. References

- Bridson, Hourihan & Nordenstam — [*Curl-Noise for Procedural Fluid Flow*](https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph2007-curlnoise.pdf) (SIGGRAPH 2007) — the origin.
- Emil Dziewanowski — [*Dissecting Curl Noise*](https://emildziewanowski.com/curl-noise/) — 3D curl formula, tileable vector noise, force-field caching, SDF surface-flow.
- al-ro — [*3D Curl Noise*](https://al-ro.github.io/projects/particles/) — the cross-product-of-gradients method (Method B).
- [Elementary potential flows & superposition](https://eng.libretexts.org/Bookshelves/Civil_Engineering/Intermediate_Fluid_Mechanics_(Liburdy)/06:_Potential_Flows) — vortex, sink, the spiral sink.
- Sucker Punch — [*Blowing from the West: Simulating Wind in Ghost of Tsushima*](https://gdcvault.com/play/1027124/Blowing-from-the-West-Simulating) and [the "vorticles" writeup](https://www.gamedeveloper.com/design/using-vorticles-to-simulate-wind-in-i-ghost-of-tsushima-i-) — layered ambient + sparse local disturbances at open-world scale.
- NVIDIA — [*GPU Gems 3, Ch. 30: Real-Time Simulation and Rendering of 3D Fluids*](https://developer.nvidia.com/gpugems/gpugems3/part-v-physics-simulation/chapter-30-real-time-simulation-and-rendering-3d-fluids) — advection, the pressure solve we're skipping.
- JangaFX — [*Curl Noise* production insights](https://jangafx.com/insights/curl-noise) — blurry inputs make better swirls; layer octaves; rotate the gradient off 90° for push/pull.
- [Unity.Mathematics `noise` API](https://docs.unity3d.com/Packages/com.unity.mathematics@1.3/api/Unity.Mathematics.noise.html) — `snoise(float3, out float3 gradient)` gives the analytic gradient; `pnoise` is periodic.
