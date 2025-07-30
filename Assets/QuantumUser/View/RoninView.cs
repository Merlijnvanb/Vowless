namespace Quantum
{
    using UnityEngine;

    public class RoninView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Transform Body;

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin))
                return;

            var pos = ronin.Position.ToUnityVector2();
            var rot = ronin.FacingSign > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            Body.SetPositionAndRotation(pos, rot);
        }
    }
}
