namespace Quantum
{
    using UnityEngine;
    using System.Collections.Generic;
    using TMPro;
    
    public class DebugUiView : QuantumSceneViewComponent
    {
        [System.Serializable]
        public struct Element
        {
            public TextMeshProUGUI Text;
            public string Description;
        }
        
        [System.Serializable]
        public struct PlayerElements
        {
            public Element InputLookVector;
        }

        [System.Serializable]
        public struct RoninElements
        {
            public Element Devotion;
            public Element TargetingSign;
            public Element FacingSign;
            public Element CurrentState;
            public Element StateFrame;
        }

        [System.Serializable]
        public struct SaberElements
        {
            public Element Direction;
            public Element AnimationId;
            public Element AnimFrameIndex;
            public Element CurrentState;
            public Element StateFrame;
        }
        
        [System.Serializable]
        public struct DebugUiElements
        {
            public PlayerElements Player;
            public RoninElements Ronin;
            public SaberElements Saber;
        }
        
        public int EntityIndex;
        public DebugUiElements Elements;
        
        private List<EntityRef> entities;
        private bool isP1;

        void Start()
        {
            entities = new List<EntityRef>();
            isP1 = EntityIndex == 1;
        }

        void LateUpdate()
        {
            PredictedFrame.GetAllEntityRefs(entities);

            foreach (var entity in entities)
            {
                if (entity.Index == EntityIndex)
                {
                    UpdateDebugUI(entity);
                    break;
                }
            }
        }

        private void UpdateDebugUI(EntityRef entity)
        {
            if (PredictedFrame.TryGet<PlayerData>(entity, out var playerData))
            {
                var pElements = Elements.Player;

                ApplyData(pElements.InputLookVector, playerData.InputLookDirectionVector.ToUnityVector2().ToString("#.00"));
            }

            if (PredictedFrame.TryGet<RoninData>(entity, out var roninData))
            {
                var rElements = Elements.Ronin;
                
                ApplyData(rElements.Devotion, roninData.Devotion.ToString());
                ApplyData(rElements.TargetingSign, roninData.TargetingSign.ToString());
                ApplyData(rElements.FacingSign, roninData.FacingSign.ToString());
                ApplyData(rElements.CurrentState, PredictedFrame.FindAsset(roninData.CurrentState).name);
                ApplyData(rElements.StateFrame, roninData.StateFrame.ToString());
            }

            if (PredictedFrame.TryGet<SaberData>(entity, out var saberData))
            {
                var sElements = Elements.Saber;
                
                ApplyData(sElements.Direction, saberData.Direction.Id.ToString());
                ApplyData(sElements.AnimationId, saberData.CurrentAnimationID.ToString());
                ApplyData(sElements.AnimFrameIndex, saberData.CurrentAnimationFrameIndex.ToString());
                ApplyData(sElements.CurrentState, PredictedFrame.FindAsset(saberData.CurrentState).name);
                ApplyData(sElements.StateFrame, saberData.StateFrame.ToString());
            }
        }

        private void ApplyData(Element element, string value)
        {
            string input;
            
            if (isP1)
            {
                input = element.Description + ": " + value;
            }
            else
            {
                input = value + " :" + element.Description;
            }
            
            element.Text.SetText(input);
        }
    }
}
