using UnityEngine;

public enum AnimationID // THIS SCRIPT IS NOT USED PLEASE DO NOT CONFUSE THANK YOU
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
    public bool UseBaseHilt;
    public bool UseBaseBlade;
    public Mesh HiltMesh;
    public Mesh BladeMesh;
    public int TicksActive;
}

[CreateAssetMenu(menuName = "AnimationSystem/AnimationData")]
public class AnimationData : ScriptableObject
{
    public AnimationID ID;
    public FrameData[] Frames;
    public bool Loop;
    [HideInInspector] public int TotalDuration;
}
