using UnityEngine;

public class ClothSim : MonoBehaviour
{
    public Vector3 EndPoint;
    public int Resolution = 5;
    public Vector3 GravityVector;
    public Vector3 WindVector;
    public float Damping = 1f;
    public int SubSteps = 1;
    public int Iterations = 5;
    public int SimStepPerSecond = 60;
    public float MaxFrameTime = 0.05f;
    
    private Point[] points;
    private Constraint[] constraints;
    private float accumulator;
    private float simStep;
    
    private class Point
    {
        public Vector3 Pos;
        public Vector3 PreviousPos;
        public bool IsPinned;
    }

    private struct Constraint
    {
        public int A, B;
        public float RestLength;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = new Point[Resolution];
        constraints = new Constraint[Resolution - 1];
        accumulator = 0;
        simStep = 1f / SimStepPerSecond;
        
        var startToEnd = EndPoint - transform.position;

        for (int i = 0; i < Resolution; i++)
        {
            var point = new Point();
            
            var pos = transform.position + startToEnd / (Resolution - 1) * i;
            point.Pos = pos;
            point.PreviousPos = pos;
            point.IsPinned = i == 0; //| i == Resolution - 1;
            
            points[i] = point;
        }

        for (int i = 0; i < Resolution - 1; i++)
        {
            var constraint = new Constraint();

            var a = points[i];
            var b = points[i + 1];
            var length = Vector3.Distance(b.Pos, a.Pos);

            constraint.A = i;
            constraint.B = i + 1;
            constraint.RestLength = length;
            
            constraints[i] = constraint;
        }
    }

    // Update is called once per frame
    void Update()
    {
        points[0].Pos = transform.position;

        accumulator += Mathf.Min(Time.deltaTime, MaxFrameTime);
        while (accumulator >= simStep)
        {
            SimulateStep(simStep);
            accumulator -= simStep;
        }
    }

    private void SimulateStep(float dt)
    {
        dt /= SubSteps;
        
        for (int i = 0; i < SubSteps; i++)
        {
            foreach (var point in points)
            {
                Integrate(point, dt);
            }

            for (int j = 0; j < Iterations; j++)
            {
                foreach (var constraint in constraints)
                {
                    Constrain(constraint);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (points == null) return;
        
        foreach (var point in points)
        {
            Gizmos.DrawSphere(point.Pos, 0.01f);
        }

        foreach (var constraint in constraints)
        {
            Gizmos.DrawLine(points[constraint.A].Pos, points[constraint.B].Pos);
        }
    }

    private void Integrate(Point point, float dt)
    {
        if (point.IsPinned) return;

        var acceleration = WindVector + GravityVector;
        var velocity = (point.Pos - point.PreviousPos) * Damping;
        var newPos = point.Pos + velocity + acceleration * (dt * dt);
        point.PreviousPos = point.Pos;
        point.Pos = newPos;
    }

    private void Constrain(Constraint constraint)
    {
        var a = points[constraint.A];
        var b = points[constraint.B];
        
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
