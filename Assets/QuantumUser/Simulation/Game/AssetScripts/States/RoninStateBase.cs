namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;
    
    public abstract unsafe class RoninStateBase : AssetObject
    {
        public bool AlwaysCancelable = false;

        public virtual void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame = 0;
            
            SetSaberState(frame, entity);
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

        protected virtual RoninStateBase GetNextState(Frame frame, EntityRef entity)
        {
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var input = InputUtils.GetInput(player);
            
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var constants = frame.FindAsset<RoninConstants>(ronin->Constants);
            
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);

            if (input.Attack)
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
            if (input.Turn)
                return constants.States.TurningState;
            
            if (input.MoveDir.X != 0)
                return constants.States.WalkState;

            return constants.States.IdleState;
        }

        protected virtual void SetSaberState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var constants = frame.FindAsset<SaberConstants>(saber->Constants);

            var state = constants.States.Holding;
            
            if (saber->CurrentState != state)
                frame.Signals.OnSwitchSaberState(entity, state);
        }
    }
}