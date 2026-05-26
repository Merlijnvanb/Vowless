using UnityEngine;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Polygon;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CurveRenderer : MonoBehaviour
{
    public FrameCurveContainer CurrentCurve;

    private Vector3[] points;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private Vector2[] projectedPoints = new Vector2[48];
    private Coordinate[] coords;
    private int[] tris = new int[138];
    private GeometryFactory factory = new GeometryFactory();
    private Dictionary<(double, double), int> indexLookup = new Dictionary<(double, double), int>(48);

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;

        coords = new Coordinate[49];
        for (int i = 0; i < 49; i++)
            coords[i] = new Coordinate();
    }

    void FixedUpdate()
    {
        transform.localPosition = CurrentCurve.Origin;
        transform.localEulerAngles = CurrentCurve.Rotation;
        points = CurrentCurve.Points;

        GenerateMesh();
    }

    void OnDrawGizmos()
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
        for (int i = 0; i < 48; i++)
        {
            int src = reversed ? (47 - i) : i;
            coords[i].X = projectedPoints[src].x;
            coords[i].Y = projectedPoints[src].y;
            indexLookup[(coords[i].X, coords[i].Y)] = src;
        }
        coords[48].X = coords[0].X;
        coords[48].Y = coords[0].Y;

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

        mesh.Clear();
        mesh.vertices = points;
        mesh.triangles = tris;
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
