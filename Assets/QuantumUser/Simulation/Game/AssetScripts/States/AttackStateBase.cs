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

        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame = 0;
            
            SetSaberState(frame, entity);
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
            }
        }

        protected override void SetSaberState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var constants = frame.FindAsset<SaberConstants>(saber->Constants);

            var state = constants.States.Attacking;
            
            if (saber->CurrentState != state)
                frame.Signals.OnSwitchSaberState(entity, state);
        }

        public virtual bool IsActive(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            
            return ronin->StateFrame > StartupFrames && ronin->StateFrame <= StartupFrames + ActiveFrames;
        }
    }
}
