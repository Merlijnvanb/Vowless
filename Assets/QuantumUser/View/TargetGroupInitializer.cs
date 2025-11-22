using UnityEngine;
using Quantum;
using Unity.Cinemachine;

public class TargetGroupInitializer : QuantumSceneViewComponent
{
    public CinemachineTargetGroup TargetGroup;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QuantumEvent.Subscribe<EventOnSpawnedEntity>(listener: this, handler: AddToTargetGroup);
    }

    void AddToTargetGroup(EventOnSpawnedEntity e)
    {
        
    }
}
