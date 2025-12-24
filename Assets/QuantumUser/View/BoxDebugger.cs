namespace Quantum
{
    using UnityEngine;
    using Quantum;
    using Photon.Deterministic;
    using System.Collections.Generic;

    public class BoxDebugger : QuantumSceneViewComponent
    {
        [System.Serializable]
        public struct DebugData
        {
            public bool Hurtboxes;
            public bool Hitboxes;
        }

        [SerializeField]
        private DebugData debugData;

        private List<EntityRef> entities;

        void Start()
        {
            entities = new List<EntityRef>();
        }

        void LateUpdate()
        {
            PredictedPreviousFrame.GetAllEntityRefs(entities);

            foreach (var entity in entities)
            {
                if (PredictedPreviousFrame.TryGet<RoninData>(entity, out var ronin))
                {
                    UpdateBoxes(ronin);
                }
            }
        }

        private void UpdateBoxes(RoninData ronin)
        {
            if (debugData.Hurtboxes)
            {
                if (PredictedPreviousFrame.TryResolveList(ronin.HurtBoxes, out var hurtboxes))
                {
                    for (int i = 0; i < hurtboxes.Count; i++)
                    {
                        var hurtbox = hurtboxes[i];
                        
                        DrawRect(hurtbox.Rect.Position.ToUnityVector2(), hurtbox.Rect.WidthHeight.ToUnityVector2(), Color.blue);
                    }
                }
            }

            if (PredictedPreviousFrame.TryResolveList(ronin.HitBoxes, out var hitboxes))
            {
                for (int i = 0; i < hitboxes.Count; i++)
                {
                    var hitbox = hitboxes[i];

                    if (debugData.Hitboxes)
                    {
                        DrawRect(hitbox.Rect.Position.ToUnityVector2(), hitbox.Rect.WidthHeight.ToUnityVector2(), Color.red);
                    }
                }
            }
        }

        private void DrawRect(Vector2 pos, Vector2 wh, Color color)
        {
            var min = new Vector2(pos.x - wh.x / 2, pos.y - wh.y / 2);
            var max = new Vector2(pos.x + wh.x / 2, pos.y + wh.y / 2);
            
            Debug.DrawLine(min, new Vector3(min.x, max.y), color);
            Debug.DrawLine(new Vector3(min.x, max.y), max, color);
            Debug.DrawLine(max, new Vector3(max.x, min.y), color);
            Debug.DrawLine(min, new Vector3(max.x, min.y), color);
        }
    }
}
