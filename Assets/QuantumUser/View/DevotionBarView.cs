namespace Quantum
{
    using UnityEngine;

    public class DevotionBarView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public RectTransform BarRect;
        
        public override void OnUpdateView()
        {
            if (!PredictedPreviousFrame.TryGet<RoninData>(EntityRef, out var ronin) || 
                !PredictedPreviousFrame.TryFindAsset(PredictedPreviousFrame.RuntimeConfig.GameConfig, out var config))
                return;
            
            var devotion = ronin.Devotion.AsFloat;
            var normalized = devotion / config.DevotionMax.AsFloat;
            BarRect.localScale = new Vector3(normalized, BarRect.localScale.y, BarRect.localScale.z);
        }
    }
}
