namespace Quantum
{
    using Photon.Deterministic;
    
    public unsafe class IdleDirection : SaberStateBase
    {
        public override void EnterState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame = 0;
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame++;
            
            saber->Direction = GetDirection(frame, entity, saber->Direction);
        }

        private FPVector2 GetDirection(Frame frame, EntityRef entity, FPVector2 dir)
        {
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var input = InputUtils.GetInput(player);

            if (input.LookDir != FPVector2.Zero && input.LookDir != saber->Direction)
            {
                return input.LookDir;
            }

            return dir;
        }
    }
}