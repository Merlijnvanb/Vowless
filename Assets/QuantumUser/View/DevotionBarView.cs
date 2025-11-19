namespace Quantum
{
    using UnityEngine;

    public class DevotionBarView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public RectTransform BarRect;
        
        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin) || 
                !PredictedFrame.TryFindAsset(PredictedFrame.RuntimeConfig.GameConfig, out var config))
                return;
            
            var devotion = ronin.Devotion.AsFloat;
            var normalized = devotion / config.DevotionMax.AsFloat;
            BarRect.localScale = new Vector3(normalized, BarRect.localScale.y, BarRect.localScale.z);
        }
    }
}
