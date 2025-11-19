using UnityEngine;

[ExecuteAlways]
public class IkSystem : MonoBehaviour
{
    public Transform Root;
    public Transform Middle;
    public Transform Effector;
    public Transform Target;
    public Transform Pole;
    public float Segment0Length = 1f;
    public float Segment1Length = 1f;
    public int UpdatesPerSecond = 60;
    
    private float updateTimer = 0f;
    

    void Update()
    {
        if (updateTimer > 0f)
        {
            updateTimer -= Time.deltaTime;
            return;
        }

        updateTimer = 1f / UpdatesPerSecond;
        SolveIK();
    }

    private void SolveIK()
    {
        var r = Root.position;
        var p = Pole.position;
        var t = Target.position;
        
        var len0 = Segment0Length;
        var len1 = Segment1Length;

        var maxReach = len0 + len1;
        var dist = Mathf.Min(Vector3.Distance(r, t), maxReach);

        var toTargetDir = (t - r).normalized;
        var toPoleDir = (p - r).normalized;
        var poleProject = toPoleDir - Vector3.Dot(toPoleDir, toTargetDir) * toTargetDir;
        poleProject.Normalize();
        
        float cosTheta = (len0*len0 + dist*dist - len1*len1) / (2 * len0 * dist);
        cosTheta = Mathf.Clamp(cosTheta, -1f, 1f);
        float theta = Mathf.Acos(cosTheta);
        
        var midPos = r + toTargetDir * (len0 * cosTheta) + poleProject * (len0 * Mathf.Sin(theta));
        var endPos = r + toTargetDir * dist;
        
        ApplyIK(midPos, endPos);
    }

    private void ApplyIK(Vector3 newMid, Vector3 newEnd)
    {
        Middle.position = newMid;
        Effector.position = newEnd;
    }
}
