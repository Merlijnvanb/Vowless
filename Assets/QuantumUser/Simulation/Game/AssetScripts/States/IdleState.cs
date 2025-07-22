namespace Quantum
{
    using Photon.Deterministic;
    
    public unsafe class IdleState : StateBase
    {
        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);

            ronin->StateFrame = 0;
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
            
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            
            var nextState = GetNextState(frame, entity);
            if (nextState != this)
            {
                Log.Debug("Signal sent from entity: " + entity + ", nextState: " + nextState.name);
                frame.Signals.OnSwitchState(entity, nextState);
            }
        }
    }
}
