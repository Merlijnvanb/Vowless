namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class AttackStateBase : RoninStateBase
    {
        public AttackHeight Height;
        public FP Range;

        public int StartupFrames;
        public int ActiveFrames;
        public int RecoveryFrames;
        
        

        public int ReceivedBlockStun;

        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame = 0;
            ronin->HasHit = false;
            
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var saberConstants = frame.FindAsset(saber->Constants);

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
            
            var input = InputUtils.GetInput(frame, entity);
            if (input.Turn.WasPressed)
            {
                foreach (var window in TurnCancelWindows)
                {
                    if (ronin->StateFrame >= window.StartEndFrame.X && ronin->StateFrame < window.StartEndFrame.Y)
                    {
                        if (input.MoveDir.X == ronin->FacingSign)
                            frame.Signals.OnSwitchRoninState(entity, rConstants.States.TurningStateForward);
                        else if (input.MoveDir.X == -ronin->FacingSign)
                            frame.Signals.OnSwitchRoninState(entity, rConstants.States.TurningStateBackward);
                        else
                            frame.Signals.OnSwitchRoninState(entity, rConstants.States.TurningState);
                
                        var saber = frame.Unsafe.GetPointer<SaberData>(entity);
                        var saberConstants = frame.FindAsset(saber->Constants);
            
                        if (saber->CurrentState != saberConstants.States.Holding)
                        {
                            frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Holding);
                        }
                    }
                }
            }
            
            var totalFrames = StartupFrames + ActiveFrames + RecoveryFrames;
            if (ronin->StateFrame > totalFrames)
            {
                var nextState = GetNextState(frame, entity);
                frame.Signals.OnSwitchRoninState(entity, nextState);
                
                var saber = frame.Unsafe.GetPointer<SaberData>(entity);
                var saberConstants = frame.FindAsset(saber->Constants);
            
                if (saber->CurrentState != saberConstants.States.Holding)
                {
                    frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Holding);
                }
            }
        }

        public virtual bool IsActive(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            
            return ronin->StateFrame > StartupFrames && ronin->StateFrame <= StartupFrames + ActiveFrames;
        }
    }
}
