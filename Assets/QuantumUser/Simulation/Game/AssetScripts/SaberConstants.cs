namespace Quantum
{
    using Photon.Deterministic;

    public class SaberConstants : AssetObject
    {
        [System.Serializable]
        public struct StateData
        {
            public IdleDirection IdleDirection;
        }

        public StateData States;
    }
}