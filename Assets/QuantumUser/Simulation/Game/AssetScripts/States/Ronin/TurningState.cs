namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class TurningState : RoninStateBase
    {
        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            
            ronin->StateContext.StateFrame = 0;
            ronin->FacingSign *= -1;

            frame.Events.VfxTurnRegular(entity);
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateContext.StateFrame++;
            
            if (ronin->StateContext.StateFrame > Duration)
            {
                var nextState = GetNextState(frame, entity);
                frame.Signals.OnSwitchRoninState(entity, nextState);
            }
        }
    }
}