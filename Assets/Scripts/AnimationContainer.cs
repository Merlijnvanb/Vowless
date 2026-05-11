using System;
using UnityEngine;
using System.Collections.Generic;
using Quantum;

[CreateAssetMenu(fileName = "AnimationContainer", menuName = "Scriptable Objects/AnimationContainer")]
public class AnimationContainer : ScriptableObject
{
    [SerializeField] private AnimationData[] full;
    [SerializeField] private AnimationData[] lower;
    [SerializeField] private AnimationData[] upper;

    private Dictionary<AnimationID, AnimationData> _lower = new();
    private Dictionary<AnimationID, AnimationData> _upper = new();
    private Dictionary<(AnimationID, SaberDirection), AnimationData> _lowerDirDep = new();
    private Dictionary<(AnimationID, SaberDirection), AnimationData> _upperDirDep = new();

    public AnimationData GetLower(AnimationID id, SaberDirection direction)
    {
        if (_lowerDirDep.TryGetValue((id, direction), out var anim)) return anim;
        return _lower[id];
    }
    
    public AnimationData GetUpper(AnimationID id, SaberDirection direction)
    {
        if (_upperDirDep.TryGetValue((id, direction), out var anim)) return anim;
        return _upper[id];
    }

    void OnEnable()
    {
        foreach (var anim in full ?? Array.Empty<AnimationData>())
        {
            if (anim.Info.IsSaberDirDependent)
            {
                if (!_lowerDirDep.TryAdd((anim.Info.ID, anim.Info.SaberDirection), anim))
                {
                    Debug.Log("Error adding id: " + anim.Info.ID + " with dir:" + anim.Info.SaberDirection + " to _lowerDirDep dictionary.");
                }
                
                if (!_upperDirDep.TryAdd((anim.Info.ID, anim.Info.SaberDirection), anim))
                {
                    Debug.Log("Error adding id: " + anim.Info.ID + " with dir:" + anim.Info.SaberDirection + " to _upperDirDep dictionary.");
                }

                continue;
            }
            
            if (!_lower.TryAdd(anim.Info.ID, anim))
                Debug.Log("Error adding id: " + anim.Info.ID + " to _lower dictionary.");
            
            if (!_upper.TryAdd(anim.Info.ID, anim))
                Debug.Log("Error adding id: " + anim.Info.ID + " to _upper dictionary.");
        }
        
        foreach (var anim in lower ?? Array.Empty<AnimationData>())
        {
            if (anim.Info.IsSaberDirDependent)
            {
                if (!_lowerDirDep.TryAdd((anim.Info.ID, anim.Info.SaberDirection), anim))
                {
                    Debug.Log("Error adding id: " + anim.Info.ID + " with dir:" + anim.Info.SaberDirection + " to _lowerDirDep dictionary.");
                }

                continue;
            }
            
            if (!_lower.TryAdd(anim.Info.ID, anim))
                Debug.Log("Error adding id: " + anim.Info.ID + " to _lower dictionary.");
        }
        
        foreach (var anim in upper ?? Array.Empty<AnimationData>())
        {
            if (anim.Info.IsSaberDirDependent)
            {
                if (!_upperDirDep.TryAdd((anim.Info.ID, anim.Info.SaberDirection), anim))
                {
                    Debug.Log("Error adding id: " + anim.Info.ID + " with dir:" + anim.Info.SaberDirection + " to _upperDirDep dictionary.");
                }

                continue;
            }
            
            if (!_upper.TryAdd(anim.Info.ID, anim))
                Debug.Log("Error adding id: " + anim.Info.ID + " to _upper dictionary.");
        }
    }
}
