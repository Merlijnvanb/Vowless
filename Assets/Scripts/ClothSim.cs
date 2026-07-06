using System.Linq;
using UnityEngine;
using Unity.Mathematics;

public class ClothSim : MonoBehaviour
{
    [System.Serializable]
    public struct EditorOriginPoint
    {
        public Transform Transform;
        public float PointsDistance;
        public int PointsAmount;
    }

    [Header("Geometry")] 
    public EditorOriginPoint[] OriginPoints;
    public int MainIndex = 0;

    [Header("Forces")]
    public Vector3 GravityVector;
    public Vector3 WindVector;
    public float MainPullFactor = 1f;
    public float Damping = 1f;

    [Header("Solver")]
    public int SubSteps = 1;
    public int Iterations = 5;

    [Header("Timing")]
    public int SimRatePerSecond = 60;
    public int RenderRatePerSecond = 24;
    public float MaxFrameTime = 0.05f;

    // Simulation state
    private Point[][] points; // ooit nog terug naar flat array van struct points met index offsets, deze classes cachen niet
    private Constraint[][] constraints;
    private float simAccumulator;
    private float simStep;

    // Render state
    private Vector3[][] renderPoints;
    private float renderAccumulator;
    private float renderStep;
    
    private class Point
    {
        public Vector3 Pos;
        public Vector3 PreviousPos;
        public bool IsPinned;
    }

    private struct Constraint
    {
        public int2 A, B;
        public float RestLength;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = new Point[OriginPoints.Length][];
        constraints = new Constraint[OriginPoints.Length][];
        renderPoints = new Vector3[OriginPoints.Length][];
        
        for (int i = 0; i < points.Length; i++)
        {
            var origin = OriginPoints[i];
            points[i] = new Point[origin.PointsAmount];
            constraints[i] = new Constraint[origin.PointsAmount - 1];
            renderPoints[i] = new Vector3[origin.PointsAmount];

            for (int j = 0; j < origin.PointsAmount; j++)
            {
                var point = new Point();
                
                var pos = origin.Transform.position + origin.PointsDistance * Vector3.down * j;
                point.Pos = pos;
                point.PreviousPos = pos;
                point.IsPinned = j == 0; //| i == Resolution - 1;
            
                points[i][j] = point;

                if (j >= origin.PointsAmount - 1) continue;
                
                var constraint = new Constraint();
                
                constraint.A = new int2(i, j);
                constraint.B = new int2(i, j + 1);
                constraint.RestLength = origin.PointsDistance;
                
                constraints[i][j] = constraint;
            }
        }
        
        simAccumulator = 0;
        renderAccumulator = 0;
        simStep = 1f / SimRatePerSecond;
        renderStep = 1f / RenderRatePerSecond;
        
        FillRenderPoints();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i][0].Pos = OriginPoints[i].Transform.position;
        }
        
        
        var frameTime = Mathf.Min(Time.deltaTime, MaxFrameTime); // ooit nog even kijken naar anchor point interpolation

        simAccumulator += frameTime;
        while (simAccumulator >= simStep)
        {
            SimulateStep(simStep);
            simAccumulator -= simStep;
        }

        renderAccumulator += frameTime;
        if (renderAccumulator >= renderStep)
        {
            FillRenderPoints();
            while (renderAccumulator >= renderStep)
                renderAccumulator -= renderStep;
        }
    }

    private void FillRenderPoints()
    {
        for (int i = 0; i < points.Length; i++)
        {
            for (int j = 0; j < points[i].Length; j++)
            {
                renderPoints[i][j] = points[i][j].Pos;
            }
        }
    }

    private void SimulateStep(float dt)
    {
        dt /= SubSteps;
        
        for (int i = 0; i < SubSteps; i++)
        {
            for (int j = 0; j < points.Length; j++)
            {
                for (int k = 0; k < points[j].Length; k++)
                {
                    var point = points[j][k];
                    var acceleration = WindVector + GravityVector;

                    if (j < points.Length - 1)
                    {
                        var endPoint = points[j + 1][0];
                        var t = (float)k / points[j].Length;
                        acceleration = Vector3.Lerp((endPoint.Pos - point.Pos) * MainPullFactor, acceleration, t);
                    }
                    
                    Integrate(point, acceleration, dt);
                }
            }

            for (int j = 0; j < Iterations; j++)
            {
                for (int k = 0; k < constraints.Length; k++)
                {
                    for (int l = 0; l < constraints[k].Length; l++)
                    {
                        Constrain(constraints[k][l]);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (renderPoints == null) return;
        
        for (int i = 0; i < renderPoints.Length; i++)
        {
            for (int j = 0; j < renderPoints[i].Length; j++)
            {
                Gizmos.DrawSphere(renderPoints[i][j], 0.01f);
            }
        }

        for (int i = 0; i < constraints.Length; i++)
        {
            for (int j = 0; j < constraints[i].Length; j++)
            {
                var c = constraints[i][j];
                Gizmos.DrawLine(renderPoints[c.A.x][c.A.y], renderPoints[c.B.x][c.B.y]);
            }
        }
    }

    private void Integrate(Point point, Vector3 acceleration, float dt)
    {
        if (point.IsPinned) return;
        
        var velocity = (point.Pos - point.PreviousPos) * Damping;
        var newPos = point.Pos + velocity + acceleration * (dt * dt);
        point.PreviousPos = point.Pos;
        point.Pos = newPos;
    }

    private void Constrain(Constraint constraint)
    {
        var a = points[constraint.A.x][constraint.A.y];
        var b = points[constraint.B.x][constraint.B.y];
        
        var delta = b.Pos - a.Pos;
        var dist = delta.magnitude;
        
        if (dist == 0)
            return;
        
        var diff = (dist - constraint.RestLength) / dist;
        var correction = 0.5f * diff * delta;

        if (a.IsPinned && b.IsPinned) return;
        
        if (a.IsPinned)
        {
            b.Pos -= correction * 2;
            return;
        }
        
        if (b.IsPinned)
        {
            a.Pos += correction * 2;
            return;
        }
        
        a.Pos += correction;
        b.Pos -= correction;
    }
}
