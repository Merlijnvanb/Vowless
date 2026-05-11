using UnityEngine;
using Unity.Mathematics;
using Quantum;

public enum AnimationType
{
    Full,
    Upper,
    Lower
}

public enum AnimatableElement // (edit: misschien ooit nog) UITSTEEKSEL DINGETJE UIT HAND MAAK HET IN BLENDER NIET VERGETEN (geen menselijke hand maar een soort blobje)
{
    Legs,
    Arms,
    Head,
    //HandL,
    //HandR,
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
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;
    public Vector3 LocalScale;
}

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable Objects/AnimationData")]
public class AnimationData : ScriptableObject
{
    public AnimationInfo Info;
    public FrameContainer[] Frames;
}
