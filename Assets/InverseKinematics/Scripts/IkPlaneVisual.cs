using System;
using UnityEngine;

[ExecuteAlways]
public class IkPlaneVisual : MonoBehaviour
{
    public IkSystem IkSystem;

    public Camera Cam;
    public Transform Plane0;
    public Transform Plane1;

    void Start()
    {
        Cam = Camera.main;
    }

    void Update()
    {
        if (!Cam)
            Cam = Camera.current ? Camera.current : Camera.main;
        
        var root = IkSystem.Root.position;
        var middle = IkSystem.Middle.position;
        var effector = IkSystem.Effector.position;
        
        var plane0Dir = (middle - root).normalized;
        var plane0Pos = root + plane0Dir * (IkSystem.Segment0Length / 2f);
        Plane0.position = plane0Pos;
        ApplyAlignedRotation(Plane0, plane0Pos, plane0Dir);
        
        var plane1Dir = (effector - middle).normalized;
        var plane1Pos = middle + plane1Dir * (IkSystem.Segment1Length / 2f);
        Plane1.position = plane1Pos;
        ApplyAlignedRotation(Plane1, plane1Pos, plane1Dir);
    }

    private void ApplyAlignedRotation(Transform planeTransform, Vector3 pos, Vector3 dir)
    {
        planeTransform.rotation = Quaternion.FromToRotation(Vector3.right, dir);

        var toCam = (Cam.transform.position - pos).normalized;
        var angle = Vector3.SignedAngle(planeTransform.up, toCam, planeTransform.right);

        planeTransform.Rotate(angle, 0f, 0f, Space.Self);
    }
}
