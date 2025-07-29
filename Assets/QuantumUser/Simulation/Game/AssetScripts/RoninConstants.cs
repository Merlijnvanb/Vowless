namespace Quantum
{
    using Photon.Deterministic;

    public class RoninConstants : AssetObject
    {
        public FP PushRange;
        
        [System.Serializable]
        public struct StateData
        {
            public IdleState IdleState;
            public WalkState WalkState;
            public TurningState TurningState;
        }

        [System.Serializable]
        public struct AttackData
        {
            public ForwardHigh ForwardHigh;
            public ForwardMid ForwardMid;
            public ForwardLow ForwardLow;
            
            public BackwardHigh BackwardHigh;
            public BackwardMid BackwardMid;
            public BackwardLow BackwardLow;
        }
        
        public StateData States;
        public AttackData Attacks;
    }
}
