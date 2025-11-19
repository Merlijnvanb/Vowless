namespace Quantum
{
    using Photon.Deterministic;
    using Quantum.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SaberConstants : AssetObject
    {
        [System.Serializable]
        public struct StateData
        {
            public Holding Holding;
            public Attacking Attacking;
            public BlockStun BlockStun;
        }
        
        public StateData States;

        [System.Serializable]
        public struct SaberDirectionEditorData
        {
            public SaberDirection Id;
            public FPVector2 Vector;
            public List<BoxRect> Boxes;
        }
        
        public Dictionary<SaberDirection, SaberDirectionData> DirectionData;
        [SerializeField] private SaberDirectionEditorData[] directionData;
        
        public Dictionary<AnimationID, SaberAnimationData> SaberAnimations;
        [SerializeField] private SaberAnimationData[] saberAnimations;
        
        public FP InputMagnitudeThreshold;
        public FPVector2 SaberStartEndLength;
        public FP SaberRadius;

        public void InitData(Frame frame)
        {
            DirectionData = new Dictionary<SaberDirection, SaberDirectionData>();

            foreach (var data in directionData)
            {
                var boxes = frame.AllocateList<BoxRect>();
                
                foreach (var box in data.Boxes)
                {
                    boxes.Add(box);
                }
                
                var newData = new SaberDirectionData()
                {
                    Id = data.Id,
                    Vector = data.Vector,
                    Boxes = boxes,
                };
                
                DirectionData.TryAdd(data.Id, newData);
            }
            
            SaberAnimations = new Dictionary<AnimationID, SaberAnimationData>();
            
            foreach (var anim in saberAnimations)
            {
                SaberAnimations.TryAdd(anim.ID, anim);
            }
        }
    }
}