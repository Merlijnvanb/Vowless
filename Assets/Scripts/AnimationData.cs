using UnityEngine;
using Unity.Mathematics;
using Quantum;

public enum AnimationType
{
    Full,
    Upper,
    Lower
}

public enum AnimatableElement
{
    Legs,
    Arms,
    HandL,
    HandR,
    Handle,
    Blade
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
    public FrameElement[] FrameElements;
    public int2 Span;
}

[System.Serializable]
public struct FrameElement
{
    public AnimatableElement Element;

    public Mesh Mesh;
    public Vector3 Position;
    public Quaternion Rotation;
}

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable Objects/AnimationData")]
public class AnimationData : ScriptableObject
{
    public AnimationInfo Info;
    public FrameContainer[] Frames;
}
