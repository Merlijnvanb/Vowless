using System;
using System.Collections.Generic;
using UnityEngine;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Polygon;

/// <summary>
/// Computes a curve's triangle index buffer (constrained Delaunay) from its points.
/// Intended to be run once at import time and baked into the asset, so nothing
/// triangulates at runtime. The result is indices into the supplied points array.
/// </summary>
public static class CurveTriangulator
{
    public static int[] Triangulate(Vector3[] points)
    {
        int n = points?.Length ?? 0;
        if (n < 3)
            return Array.Empty<int>();

        var normal = ComputeNormal(points);
        var projected = ProjectPointsTo2D(points, normal);

        bool reversed = SignedArea(projected) < 0f;

        var factory = new GeometryFactory();
        var coords = new Coordinate[n + 1];
        var indexLookup = new Dictionary<(double, double), int>(n);

        for (int i = 0; i < n; i++)
        {
            int src = reversed ? (n - 1 - i) : i;
            coords[i] = new Coordinate(projected[src].x, projected[src].y);
            indexLookup[(coords[i].X, coords[i].Y)] = src;
        }
        coords[n] = new Coordinate(coords[0].X, coords[0].Y);

        var ring = factory.CreateLinearRing(coords);
        var polygon = factory.CreatePolygon(ring);
        var triangulator = new ConstrainedDelaunayTriangulator(polygon);
        var triangles = triangulator.GetTriangles();

        var tris = new List<int>(triangles.Count * 3);
        foreach (var tri in triangles)
        {
            tris.Add(indexLookup[(tri.GetCoordinate(0).X, tri.GetCoordinate(0).Y)]);
            tris.Add(indexLookup[(tri.GetCoordinate(1).X, tri.GetCoordinate(1).Y)]);
            tris.Add(indexLookup[(tri.GetCoordinate(2).X, tri.GetCoordinate(2).Y)]);
        }

        return tris.ToArray();
    }

    private static Vector3 ComputeNormal(Vector3[] points) // Newell's algorithm
    {
        Vector3 n = Vector3.zero;
        int count = points.Length;
        for (int i = 0; i < count; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % count];
            n.x += (current.y - next.y) * (current.z + next.z);
            n.y += (current.z - next.z) * (current.x + next.x);
            n.z += (current.x - next.x) * (current.y + next.y);
        }
        return n.normalized;
    }

    private static Vector2[] ProjectPointsTo2D(Vector3[] points, Vector3 normal)
    {
        Quaternion rotation;
        if (Vector3.Dot(normal, Vector3.forward) < -0.9999f)
            rotation = Quaternion.AngleAxis(180f, Vector3.right);
        else
            rotation = Quaternion.FromToRotation(normal, Vector3.forward);

        var projected = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            var p = rotation * points[i];
            projected[i] = new Vector2(p.x, p.y);
        }
        return projected;
    }

    private static float SignedArea(Vector2[] poly)
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
