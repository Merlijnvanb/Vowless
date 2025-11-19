namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class TurningState : RoninStateBase
    {
        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            
            ronin->StateFrame = 0;
            ronin->FacingSign *= -1;
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
            
            if (ronin->StateFrame > Duration)
            {
                var nextState = GetNextState(frame, entity);
                frame.Signals.OnSwitchRoninState(entity, nextState);
            }
        }
    }
}