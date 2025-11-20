namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class StateManagerSystem : SystemMainThreadFilter<StateManagerSystem.Filter>, 
        ISignalOnSwitchRoninState, ISignalOnSwitchSaberState, 
        ISignalOnHit, ISignalOnReceivedHit
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
            
            frame.Signals.OnCalculateTarget(entity);
            
            //Log.Debug("Ronin state of " + entity + " switched to: " + stateAsset.name);
        }

        public void OnSwitchSaberState(Frame frame, EntityRef entity, AssetRef<SaberStateBase> state)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->CurrentState = state;
            
            var stateAsset = frame.FindAsset<SaberStateBase>(state);
            stateAsset.EnterState(frame, entity);
            
            //Log.Debug("Saber state of " + entity + " switched to: " + stateAsset.name);
        }

        public void OnHit(Frame frame, EntityRef entity, CombatResult result)
        {
            var attackState = frame.FindAsset(result.AttackState);
            
            switch (result.Type)
            {
                case CombatResultType.Hit:
                    Log.Debug(entity + " landed an attack");
                    break;
                
                case CombatResultType.Deflected:
                    frame.Signals.OnIncreaseDevotion(entity, attackState.DevotionGain);
                    Log.Debug(entity + " had their attack deflected");
                    break;
                
                case CombatResultType.Blocked:
                    Log.Debug(entity + " had their attack blocked");
                    break;
                
                case CombatResultType.Clashed:
                    Log.Debug(entity + "'s attack clashed");
                    break;
            }
        }

        public void OnReceivedHit(Frame frame, EntityRef entity, CombatResult result)
        {
            switch (result.Type)
            {
                case CombatResultType.Hit:
                    Log.Debug(entity + " got hit by an attack");
                    frame.Signals.OnRoninDeath(entity);
                    break;
                
                case CombatResultType.Deflected:
                    Log.Debug(entity + " deflected an attack");
                    
                    var saber = frame.Unsafe.GetPointer<SaberData>(entity);
                    var saberConstants = frame.FindAsset(saber->Constants);
                    var attackAsset = frame.FindAsset(result.AttackState);
            
                    OnSwitchSaberState(frame, entity, saberConstants.States.BlockStun);
                    saber->StateFrame = attackAsset.ReceivedBlockStun;
                    
                    break;
                
                case CombatResultType.Blocked:
                    Log.Debug(entity + " blocked an attack");
                    break;
                
                case CombatResultType.Clashed:
                    Log.Debug(entity + "'s attack clashed");
                    break;
            }
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
