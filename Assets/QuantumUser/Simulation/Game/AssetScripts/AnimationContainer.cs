namespace Quantum
{
    using Photon.Deterministic;

    public enum AnimationType
    {
        Full,
        LegsExcluded,
        LegsOnly
    }

    public class AnimationContainer : AssetObject
    {
        public AnimationType Type;
    }
}
