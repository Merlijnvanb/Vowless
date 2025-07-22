namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class StateManagerSystem : SystemMainThreadFilter<StateManagerSystem.Filter>, ISignalOnSwitchRoninState, ISignalOnSwitchSaberState
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            UpdateRoninState(frame, ref filter);
            UpdateSaberState(frame, ref filter);
        }

        private void UpdateRoninState(Frame frame, ref Filter filter)
        {
            var ronin = filter.Ronin;
            var currentState = frame.FindAsset(ronin->CurrentState);
            
            currentState.UpdateState(frame, filter.Entity);
        }

        private void UpdateSaberState(Frame frame, ref Filter filter)
        {
            var saber = filter.Saber;
            var currentState = frame.FindAsset(saber->CurrentState);
            
            currentState.UpdateState(frame, filter.Entity);
        }

        public void OnSwitchRoninState(Frame frame, EntityRef entity, AssetRef<RoninStateBase> state)
        {
            Log.Debug("entity: " + entity + ", state: " + frame.FindAsset<RoninStateBase>(state).name);
            
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            
            ronin->CurrentState = state;
            
            var stateAsset = frame.FindAsset<RoninStateBase>(state);
            stateAsset.EnterState(frame, entity);
        }

        public void OnSwitchSaberState(Frame frame, EntityRef entity, AssetRef<SaberStateBase> state)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->CurrentState = state;
            
            var stateAsset = frame.FindAsset<SaberStateBase>(state);
            stateAsset.EnterState(frame, entity);
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerData* Player;
            public RoninData* Ronin;
            public SaberData* Saber;
        }
    }
}
