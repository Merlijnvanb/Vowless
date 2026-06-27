using UnityEngine;
using Unity.Mathematics;
using Quantum;

public enum AnimationID
{
    Idle,
    Walk,
    
    TurningStationary,
    TurningForward,
    TurningBackward,
}

public enum CurveID
{
    //Persistent
    LowerBodyC,
    LowerBodyL,
    LowerBodyR,
    UpperBodyC,
    UpperBodyL,
    UpperBodyR,
    Head,
    Handle,
    Blade,
}

[System.Serializable]
public struct AnimationInfo
{
    public AnimationID ID;
    public bool IsLoop;
    public int Duration;
    
    public bool IsPartial;
    public CurveID[] PartialCurves;
    
    public bool IsSaberDirDependent;
    public SaberDirection SaberDirection;
}

[System.Serializable]
public struct FrameContainer
{
    public int2 Span;
    public FrameCurveContainer[] Persistent;
    public FrameCurveContainer[] Transient;
    public FrameCurveContainer[] Guide;
}

[System.Serializable]
public struct FrameCurveContainer
{
    public CurveID ID;
    public Vector3 Origin; // local position
    public Vector3 Rotation; // local rotation (eulerAngles)
    public Vector3[] Points; // in local space relative to origin and rotation
    public int[] Triangles; // baked triangle indices into Points (computed at import)
}

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable Objects/AnimationData")]
public class AnimationData : ScriptableObject
{
    [ReadOnly]public AnimationInfo Info;
    [ReadOnly]public FrameContainer[] Frames;
}
