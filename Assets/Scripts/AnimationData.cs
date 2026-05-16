using UnityEngine;
using Unity.Mathematics;
using Quantum;

public enum AnimationType
{
    Full,
    Upper,
    Lower
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
    
    Transient
}

[System.Serializable]
public struct AnimationInfo
{
    public AnimationID ID;
    public AnimationType Type;
    public bool IsLoop;
    public bool IsSaberDirDependent;
    public SaberDirection SaberDirection;
}

[System.Serializable]
public struct FrameContainer
{
    public int2 Span;
    public FrameCurveContainer[] Persistent;
    public FrameCurveContainer[] Transient;
}

[System.Serializable]
public struct FrameCurveContainer
{
    public CurveID ID;
    public Vector3 Origin; // local position
    public Vector3 Rotation; // local rotation in radians
    public Vector3[] Points; // in local space relative to origin and rotation
}

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable Objects/AnimationData")]
public class AnimationData : ScriptableObject
{
    public AnimationInfo Info;
    public FrameContainer[] Frames;
}
