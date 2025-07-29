namespace Quantum
{
    using Photon.Deterministic;
    
    public unsafe class IdleState : RoninStateBase
    {
        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
            
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            
            if (AlwaysCancelable)
            {
                var nextState = GetNextState(frame, entity);
                if (nextState != this)
                    frame.Signals.OnSwitchRoninState(entity, nextState);
            }
        }
    }
}
