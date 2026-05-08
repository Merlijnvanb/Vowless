namespace Quantum
{
    using Photon.Deterministic;
    using System.Collections.Generic;

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
            public AttackStateBase ForwardHigh;
            public AttackStateBase ForwardMid;
            public AttackStateBase ForwardLow;

            public AttackStateBase BackwardHigh;
            public AttackStateBase BackwardMid;
            public AttackStateBase BackwardLow;

            public AttackStateBase TurnedForwardHigh;
            public AttackStateBase TurnedForwardMid;
            public AttackStateBase TurnedForwardLow;

            public AttackStateBase TurnedBackwardHigh;
            public AttackStateBase TurnedBackwardMid;
            public AttackStateBase TurnedBackwardLow;
        }
        
        public StateData States;
        public AttackData Attacks;
    }
}
