using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CurveRenderer : MonoBehaviour
{
    private Vector3[] points;
    private int[] tris;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    // We never read the mesh back, so use a fixed, generous bounds and skip the
    // per-frame bounds recalculation on every vertex upload.
    private static readonly Bounds FixedBounds = new Bounds(Vector3.zero, Vector3.one * 100f);

    private const MeshUpdateFlags UploadFlags =
        MeshUpdateFlags.DontRecalculateBounds |
        MeshUpdateFlags.DontValidateIndices |
        MeshUpdateFlags.DontNotifyMeshUsers;

    public void Initialize(int curveSampleRate)
    {
        points = Array.Empty<Vector3>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.bounds = FixedBounds;
        meshFilter.sharedMesh = mesh;
    }

    public void UpdateMesh(FrameCurveContainer curve)
    {
        transform.localPosition = curve.Origin;
        transform.localEulerAngles = curve.Rotation;

        // Vertices and topology are independent: positions can morph (interpolation)
        // while the baked index buffer stays put. Only push each when its source changes.
        if (!ReferenceEquals(points, curve.Points))
        {
            points = curve.Points;
            mesh.SetVertices(points, 0, points.Length, UploadFlags);
        }

        if (curve.Triangles != null && !ReferenceEquals(tris, curve.Triangles))
        {
            tris = curve.Triangles;
            mesh.SetIndices(tris, 0, tris.Length, MeshTopology.Triangles, 0, calculateBounds: false);
        }

        meshRenderer.enabled = true;
    }

    public void Disable()
    {
        meshRenderer.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        if (points == null)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;

        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, 0.01f);
        }
    }
}
