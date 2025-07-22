namespace Quantum
{
    using Photon.Deterministic;

    public class RoninConstants : AssetObject
    {
        [System.Serializable]
        public struct StateData
        {
            public IdleState IdleState;
            public WalkState WalkState;
        }

        public StateData States;
    }
}
