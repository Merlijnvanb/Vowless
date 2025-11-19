namespace Quantum
{
    using Photon.Deterministic;

    public class RoninConstants : AssetObject
    {
        public FP PushRange;
        public BoxRect BaseRect;
        
        [System.Serializable]
        public struct StateData
        {
            public IdleState IdleState;
            public WalkState WalkState;
            public TurningState TurningState;
            public TurningStateForward TurningStateForward;
            public TurningStateBackward TurningStateBackward;
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
            
            
            public TurnedForwardHigh TurnedForwardHigh;
            public TurnedForwardMid TurnedForwardMid;
            public TurnedForwardLow TurnedForwardLow;
            
            public TurnedBackwardHigh TurnedBackwardHigh;
            public TurnedBackwardMid TurnedBackwardMid;
            public TurnedBackwardLow TurnedBackwardLow;
        }
        
        public StateData States;
        public AttackData Attacks;
    }
}
