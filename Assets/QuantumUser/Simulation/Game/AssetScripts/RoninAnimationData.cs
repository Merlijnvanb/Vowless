namespace Quantum
{
    using UnityEngine;
    using Photon.Deterministic;

    [System.Serializable]
    public struct RoninAnimationFrame
    {
        public Mesh BodyMesh;
        public IntVector2 StartEndFrame;
    }

    [System.Serializable]
    public class RoninAnimationData : AssetObject
    {
        public AnimationID ID;
        public RoninAnimationFrame[] Frames;
        public bool IsLoop;
    }
}
