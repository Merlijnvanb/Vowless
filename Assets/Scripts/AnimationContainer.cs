using System;
using UnityEngine;
using System.Collections.Generic;
using Quantum;

[CreateAssetMenu(fileName = "AnimationContainer", menuName = "Scriptable Objects/AnimationContainer")]
public class AnimationContainer : ScriptableObject
{
    public struct CategorizedHolder
    {
        public AnimationData[] Bases;
        public AnimationData[] Partials;
    }
    
    [SerializeField] private AnimationData[] bases;
    [SerializeField] private AnimationData[] partials;

    private Dictionary<AnimationID, CategorizedHolder> library = new Dictionary<AnimationID, CategorizedHolder>();

    public bool TryGetHolder(AnimationID id, out CategorizedHolder outHolder)
    {
        outHolder = default;
        
        if (!library.TryGetValue(id, out var holder))
            return false;
        
        outHolder = holder;
        return true;
    }

    void OnEnable()
    {
        library.Clear();

        foreach (var value in Enum.GetValues(typeof(AnimationID)))
        {
            var baseList = new List<AnimationData>();
            var partialList = new List<AnimationData>();

            foreach (var baseAnim in bases)
            {
                if (baseAnim.Info.ID != (AnimationID)value)
                    continue;
                
                baseList.Add(baseAnim);
            }

            foreach (var partialAnim in partials)
            {
                if (partialAnim.Info.ID != (AnimationID)value)
                    continue;
                
                partialList.Add(partialAnim);
            }
            
            var holder = new CategorizedHolder
            {
                Bases = baseList.ToArray(),
                Partials = partialList.ToArray()
            };

            if (!library.TryAdd((AnimationID)value, holder))
            {
                Debug.LogError($"{(AnimationID)value} already added to animation library");
            }
        }
    }
}
