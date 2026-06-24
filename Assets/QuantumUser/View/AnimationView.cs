namespace Quantum
{
    using UnityEngine;

    public class AnimationView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public AnimationContainer Container;

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin) ||
                !PredictedFrame.TryGet<SaberData>(EntityRef, out var saber))
                return;

            var roninState = PredictedFrame.FindAsset(ronin.CurrentState);
            var saberState = PredictedFrame.FindAsset(saber.CurrentState);
            
            
        }
    }
}
