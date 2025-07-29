namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class StateManagerSystem : SystemMainThreadFilter<StateManagerSystem.Filter>, 
        ISignalOnSwitchRoninState, ISignalOnSwitchSaberState, 
        ISignalOnHit, ISignalOnReceivedHit, ISignalOnDeflected, ISignalOnReceivedDeflected, ISignalOnClashed
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
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->CurrentState = state;
            
            var stateAsset = frame.FindAsset<RoninStateBase>(state);
            stateAsset.EnterState(frame, entity);
            
            Log.Debug("Ronin state of " + entity + " switched to: " + stateAsset.name);
        }

        public void OnSwitchSaberState(Frame frame, EntityRef entity, AssetRef<SaberStateBase> state)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->CurrentState = state;
            
            var stateAsset = frame.FindAsset<SaberStateBase>(state);
            stateAsset.EnterState(frame, entity);
            
            Log.Debug("Saber state of " + entity + " switched to: " + stateAsset.name);
        }

        public void OnHit(Frame frame, EntityRef entity, AssetRef<AttackStateBase> attack)
        {
            Log.Debug(entity + " landed an attack");
        }

        public void OnReceivedHit(Frame frame, EntityRef entity, AssetRef<AttackStateBase> attack)
        {
            Log.Debug(entity + " got hit");
            frame.Signals.OnRoninDeath(entity);
        }

        public void OnDeflected(Frame frame, EntityRef entity, AssetRef<AttackStateBase> attack)
        {
            Log.Debug(entity + " had their attack deflected");
        }

        public void OnReceivedDeflected(Frame frame, EntityRef entity, AssetRef<AttackStateBase> attack)
        {
            Log.Debug(entity + " deflected an attack");
        }

        public void OnClashed(Frame frame, EntityRef entity, AssetRef<AttackStateBase> attack)
        {
            Log.Debug(entity + "'s attack clashed");
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
