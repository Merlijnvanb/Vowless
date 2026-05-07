#ifndef DISTTOSEG_INCLUDED
#define DISTTOSEG_INCLUDED

void GetDistanceToSegment_float(float3 p, float3 a, float3 b, out float dist, out float3 closest)
{
    float3 ab = b - a;
    float3 ap = p - a;
    float t = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    closest = a + t * ab;
    dist = length(p - closest);
}

#endif

