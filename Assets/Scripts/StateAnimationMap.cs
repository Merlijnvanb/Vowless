using System;
using UnityEngine;
using System.Collections.Generic;
using Quantum;

[CreateAssetMenu(fileName = "StateAnimationMap", menuName = "Scriptable Objects/StateAnimationMap")]
public class StateAnimationMap : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public AssetRef<AssetObject> State; // a RoninStateBase or SaberStateBase asset
        public AnimationID Animation;
    }

    [SerializeField] private Entry[] entries;

    private Dictionary<AssetGuid, AnimationID> _map;

    public bool TryGet(AssetRef state, out AnimationID id)
    {
        EnsureBuilt();
        return _map.TryGetValue(state.Id, out id);
    }

    private void OnEnable() => _map = null; // rebuild lazily, dodge SO lifecycle timing

    private void EnsureBuilt()
    {
        if (_map != null) return;

        _map = new Dictionary<AssetGuid, AnimationID>();

        foreach (var entry in entries ?? Array.Empty<Entry>())
        {
            if (!entry.State.IsValid) continue;

            if (!_map.TryAdd(entry.State.Id, entry.Animation))
                Debug.LogError($"Duplicate state mapping for {entry.State.Id} in {name}");
        }
    }
}
