namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;
    
    public abstract unsafe class RoninStateBase : AssetObject
    {
        [System.Serializable]
        public struct TurnCancelWindow
        {
            public IntVector2 StartEndFrame;
            public FP CancelCost;
        }

        [System.Serializable]
        public struct MovementData
        {
            public IntVector2 StartEndFrame;
            public FPVector2 MoveVector;
        }
        
        public bool AlwaysCancelable = false;
        
        public TurnCancelWindow[] TurnCancelWindows;
        public MovementData[] Movements;

        public virtual void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame = 0;
        }

        public virtual void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
            
            if (AlwaysCancelable)
            {
                var nextState = GetNextState(frame, entity);
                if (nextState != this)
                    frame.Signals.OnSwitchRoninState(entity, nextState);
            }
        }

        protected virtual void ApplyMovement(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            
            foreach (var data in Movements)
            {
                if (ronin->StateFrame >= data.StartEndFrame.X && ronin->StateFrame < data.StartEndFrame.Y)
                {
                    ronin->Position += new FPVector2(
                        data.MoveVector.X * ronin->TargetingSign * frame.DeltaTime, 
                        data.MoveVector.Y * frame.DeltaTime);
                }
            }
        }

        protected virtual RoninStateBase GetNextState(Frame frame, EntityRef entity)
        {
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var input = InputUtils.GetInput(frame, entity);
            
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var constants = frame.FindAsset<RoninConstants>(ronin->Constants);
            
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);

            if (input.Attack.WasPressed)
            {
                switch (saber->Direction.Id)
                {
                    case SaberDirection.FwHigh:
                        return constants.Attacks.ForwardHigh;
                    
                    case SaberDirection.FwMid:
                        return constants.Attacks.ForwardMid;
                    
                    case SaberDirection.FwLow:
                        return constants.Attacks.ForwardLow;
                    
                    
                    case SaberDirection.BwHigh:
                        return constants.Attacks.BackwardHigh;
                    
                    case SaberDirection.BwMid:
                        return constants.Attacks.BackwardMid;
                    
                    case SaberDirection.BwLow:
                        return constants.Attacks.BackwardLow;
                }
            }
            // if (input.Block)
            //     // return block
            if (input.Turn.WasPressed)
            {
                if ((int)input.MoveDir.X == ronin->TargetingSign)
                    return constants.States.TurningStateForward;
                
                if ((int)input.MoveDir.X == -ronin->TargetingSign)
                    return constants.States.TurningStateBackward;
                
                return constants.States.TurningState;
            }
            
            if (input.MoveDir.X != 0)
                return constants.States.WalkState;

            return constants.States.IdleState;
        }
    }
}