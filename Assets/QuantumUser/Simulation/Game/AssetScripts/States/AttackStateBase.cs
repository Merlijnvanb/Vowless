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
            ronin->StateFrame++;
            
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
