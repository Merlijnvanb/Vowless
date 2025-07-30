using UnityEngine;

[CreateAssetMenu(menuName = "AnimationSystem/AnimationDataHolder")]
public class AnimDataHolder : ScriptableObject
{
    public AnimationData[] SaberAnimations;
    public AnimationData[] RoninAnimations;
    public AnimationData[] AttackAnimations;
}
