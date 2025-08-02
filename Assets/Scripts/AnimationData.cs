using UnityEngine;

public enum AnimType
{
    Saber,
    Ronin,
    Attack
}

public enum AnimationID
{
    //SABER
    HoldingFwHigh,
    HoldingFwMid,
    HoldingFwLow,
    HoldingBwHigh,
    HoldingBwMid,
    HoldingBwLow,
    
    //RONIN
    Idle,
    Walking,
    Turning,
    
    //ATTACK
    AttackFwHigh,
    AttackFwMid,
    AttackFwLow,
    AttackBwHigh,
    AttackBwMid,
    AttackBwLow,
}

[System.Serializable]
public struct FrameData
{
    public Transform HiltTransform;
    public Mesh HiltMesh;
    public Mesh BladeMesh;
    public int ActiveLength;
}

[CreateAssetMenu(menuName = "AnimationSystem/AnimationData")]
public class AnimationData : ScriptableObject
{
    public AnimationID ID;
    public FrameData[] Frames;
    public bool Loop;
}
