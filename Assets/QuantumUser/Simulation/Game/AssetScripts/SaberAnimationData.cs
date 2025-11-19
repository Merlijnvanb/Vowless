namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;
    
    [System.Serializable]
    public struct SaberAnimationFrame
    {
        public FPVector3 Position;
        public FPVector3 Rotation;
        public FPVector3 PoleLPosition;
        public FPVector3 PoleRPosition;
        public Mesh HiltMesh;
        public Mesh BladeMesh;
        public IntVector2 StartEndFrame;
    }

    [System.Serializable]
    public class SaberAnimationData : AssetObject
    {
        public AnimationID ID;
        public SaberAnimationFrame[] Frames;
        public bool IsLoop;
    }
}
