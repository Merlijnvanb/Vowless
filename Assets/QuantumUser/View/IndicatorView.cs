namespace Quantum
{
    using UnityEngine;

    public class IndicatorView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Transform Indicator;
        public float Range;

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<PlayerData>(EntityRef, out var player))
                return;
            
            var directionVector = player.InputLookDirectionVector.ToUnityVector2();
            Indicator.localPosition = new Vector3(directionVector.x * Range, directionVector.y * Range, Indicator.localPosition.z);
        }
    }
}
