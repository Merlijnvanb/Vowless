namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class TurningStateForward : RoninStateBase
    {
        public int Duration;
        
        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            
            ronin->StateFrame = 0;
            ronin->FacingSign *= -1;
            ronin->IgnoreCollision = true;
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
            
            ApplyMovement(frame, entity);
            
            if (ronin->StateFrame > Duration)
            {
                ronin->IgnoreCollision = false;
                
                var nextState = GetNextState(frame, entity);
                if (nextState != this)
                    frame.Signals.OnSwitchRoninState(entity, nextState);
            }
        }
    }
}