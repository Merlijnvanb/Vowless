using System;
using UnityEngine;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Polygon;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CurveRenderer : MonoBehaviour
{
    //public FrameCurveContainer CurrentCurve;
    private int _curveSampleRate;

    private Vector3[] points;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    private Vector2[] projectedPoints;
    private Coordinate[] coords;
    private int[] tris;
    private GeometryFactory factory = new GeometryFactory();
    private Dictionary<(double, double), int> indexLookup;

    public void Initialize(int curveSampleRate)
    {
        _curveSampleRate = curveSampleRate;

        points = Array.Empty<Vector3>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        mesh.MarkDynamic();
        meshFilter.sharedMesh = mesh;
        
        projectedPoints = new Vector2[_curveSampleRate];
        tris = new int[(_curveSampleRate - 2) * 3];
        indexLookup = new Dictionary<(double, double), int>(_curveSampleRate);

        coords = new Coordinate[_curveSampleRate + 1];
        for (int i = 0; i < _curveSampleRate + 1; i++)
            coords[i] = new Coordinate();
    }

    public void UpdateMesh(FrameCurveContainer curve)
    {
        transform.localPosition = curve.Origin;
        transform.localEulerAngles = curve.Rotation;
        points = curve.Points;

        GenerateMesh();
        
        meshRenderer.enabled = true;
    }

    public void Disable()
    {
        meshRenderer.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, 0.01f);
        }
    }

    private void GenerateMesh()
    {
        var normal = ComputeNormal();
        ProjectPointsTo2D(normal);

        bool reversed = SignedArea(projectedPoints) < 0f;

        indexLookup.Clear();
        for (int i = 0; i < _curveSampleRate; i++)
        {
            int src = reversed ? ((_curveSampleRate - 1) - i) : i;
            coords[i].X = projectedPoints[src].x;
            coords[i].Y = projectedPoints[src].y;
            indexLookup[(coords[i].X, coords[i].Y)] = src;
        }
        coords[_curveSampleRate].X = coords[0].X;
        coords[_curveSampleRate].Y = coords[0].Y;

        var ring = factory.CreateLinearRing(coords);
        var polygon = factory.CreatePolygon(ring);
        var triangulator = new ConstrainedDelaunayTriangulator(polygon);
        var triangles = triangulator.GetTriangles();

        int t = 0;
        for (int i = 0; i < triangles.Count; i++)
        {
            var tri = triangles[i];
            tris[t++] = indexLookup[(tri.GetCoordinate(0).X, tri.GetCoordinate(0).Y)];
            tris[t++] = indexLookup[(tri.GetCoordinate(1).X, tri.GetCoordinate(1).Y)];
            tris[t++] = indexLookup[(tri.GetCoordinate(2).X, tri.GetCoordinate(2).Y)];
        }

        mesh.SetVertices(points);
        mesh.SetIndices(tris, 0, t, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();
    }

    private Vector3 ComputeNormal() // Computes normal using Newell's algorithm
    {
        Vector3 n = Vector3.zero;
        int pointCount = points.Length;
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % pointCount];
            n.x += (current.y - next.y) * (current.z + next.z);
            n.y += (current.z - next.z) * (current.x + next.x);
            n.z += (current.x - next.x) * (current.y + next.y);
        }
        return n.normalized;
    }

    private void ProjectPointsTo2D(Vector3 normal)
    {
        Quaternion rotation;
        
        if (Vector3.Dot(normal, Vector3.forward) < -0.9999f)
            rotation = Quaternion.AngleAxis(180f, Vector3.right);
        else
            rotation = Quaternion.FromToRotation(normal, Vector3.forward);

        for (int i = 0; i < projectedPoints.Length; i++)
        {
            var rotatedPoint = rotation * points[i];
            projectedPoints[i] = new Vector2(rotatedPoint.x, rotatedPoint.y);
        }
    }

    private float SignedArea(Vector2[] poly)
    {
        float area = 0f;
        for (int i = 0; i < poly.Length; i++)
        {
            Vector2 a = poly[i];
            Vector2 b = poly[(i + 1) % poly.Length];
            area += (a.x * b.y) - (b.x * a.y);
        }
        return area;
    }
}
