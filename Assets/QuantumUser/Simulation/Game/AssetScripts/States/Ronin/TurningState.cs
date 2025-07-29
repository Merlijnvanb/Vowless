namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class TurningState : RoninStateBase
    {
        public int Duration;
        
        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            
            ronin->StateFrame = 0;

            ronin->Turned = !ronin->Turned;
            ronin->FacingSign *= -1;
            
            SetSaberState(frame, entity);
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
            
            if (ronin->StateFrame > Duration)
            {
                var nextState = GetNextState(frame, entity);
                if (nextState != this)
                    frame.Signals.OnSwitchRoninState(entity, nextState);
            }
        }
    }
}