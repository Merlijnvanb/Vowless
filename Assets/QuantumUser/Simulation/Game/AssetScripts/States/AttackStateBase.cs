namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class AttackStateBase : RoninStateBase
    {
        [System.Serializable]
        public struct HitBoxData
        {
            public BoxRect Rect;
            public IntVector2 StartEndFrame;
        }
        
        public AttackHeight Height;
        public HitBoxData[] HitBoxes;
        public int ReceivedBlockStun;
        public bool TurnAround;
        public SaberDirection EndingDirection;
        
        public AnimationID AnimationID;

        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame = 0;
            ronin->HasHit = false;
            
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var saberConstants = frame.FindAsset(saber->Constants);

            if (saberConstants.DirectionData.TryGetValue(EndingDirection, out var newDir))
            {
                saber->Direction = newDir;
            }

            if (saber->CurrentState != saberConstants.States.Attacking)
            {
                frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Attacking);
            }
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var rConstants =  frame.FindAsset(ronin->Constants);
            ronin->StateFrame++;
            
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var saberConstants = frame.FindAsset(saber->Constants);
            
            var input = InputUtils.GetInput(frame, entity);
            if (input.Turn.WasPressed)
            {
                foreach (var window in TurnCancelWindows)
                {
                    if (ronin->StateFrame >= window.StartEndFrame.X && ronin->StateFrame <= window.StartEndFrame.Y)
                    {
                        if (input.MoveDir.X == ronin->FacingSign)
                            frame.Signals.OnSwitchRoninState(entity, rConstants.States.TurningStateForward);
                        else if (input.MoveDir.X == -ronin->FacingSign)
                            frame.Signals.OnSwitchRoninState(entity, rConstants.States.TurningStateBackward);
                        else
                            frame.Signals.OnSwitchRoninState(entity, rConstants.States.TurningState);
            
                        if (saber->CurrentState != saberConstants.States.Holding)
                        {
                            frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Holding);
                        }
                    }
                }
            }

            if (input.Attack.WasPressed)
            {
                if (ronin->HasHit)
                {
                    if (TurnAround)
                    {
                        ronin->FacingSign *= -1;
                    }
                    
                    var nextState = GetNextState(frame, entity);
                    if (nextState is AttackStateBase)
                    {
                        frame.Signals.OnSwitchRoninState(entity, nextState);
                        frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Attacking);
                    }
                }
            }
            
            if (ronin->StateFrame > Duration)
            {
                if (TurnAround)
                {
                    ronin->FacingSign *= -1;
                }
                
                var nextState = GetNextState(frame, entity);
                frame.Signals.OnSwitchRoninState(entity, nextState);
            
                if (saber->CurrentState != saberConstants.States.Holding)
                {
                    frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Holding);
                }
            }
        }

        // public virtual bool IsActive(Frame frame, EntityRef entity)
        // {
        //     var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
        //     
        //     return ronin->StateFrame > StartupFrames && ronin->StateFrame <= StartupFrames + ActiveFrames;
        // }
    }
}
