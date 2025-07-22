namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class StateManagerSystem : SystemMainThreadFilter<StateManagerSystem.Filter>, ISignalOnSwitchState
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            var ronin = filter.RoninData;
            var currentState = frame.FindAsset(ronin->CurrentState);
            
            Log.Debug("Updating state for entity: " + filter.Entity + ", and playerRef: " + filter.PlayerData->PlayerRef);
            
            currentState.UpdateState(frame, filter.Entity);
        }

        public void OnSwitchState(Frame frame, EntityRef entity, AssetRef<StateBase> state)
        {
            Log.Debug("entity: " + entity + ", state: " + frame.FindAsset<StateBase>(state).name);
            
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            
            ronin->CurrentState = state;
            
            var stateAsset = frame.FindAsset<StateBase>(state);
            stateAsset.EnterState(frame, entity);
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerData* PlayerData;
            public RoninData* RoninData;
        }
    }
}
