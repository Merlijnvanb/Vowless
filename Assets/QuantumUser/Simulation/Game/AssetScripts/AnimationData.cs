namespace Quantum
{
    using Photon.Deterministic;

    public enum AnimationType
    {
        Full,
        LegsExcluded,
        LegsOnly
    }

    public class AnimationData : AssetObject
    {
        public AnimationType Type;
    }
}
