namespace Quantum
{
    using UnityEngine;

    public class SaberView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Transform Indicator;

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<SaberData>(EntityRef, out var saber))
                return;

            var saberDir = saber.Direction.ToUnityVector2();
            var dirVector3 = new Vector3(saberDir.x, saberDir.y, 0);
            Indicator.rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), dirVector3);
        }
    }
}
