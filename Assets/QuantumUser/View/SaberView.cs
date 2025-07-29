namespace Quantum
{
    using UnityEngine;

    public class SaberView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Transform Indicator;

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<SaberData>(EntityRef, out var saber) || !PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin))
                return;

            var saberDir = saber.Direction.Vector.ToUnityVector2();
            var signedDir = new Vector2(saberDir.x * ronin.FacingSign, saberDir.y);
            
            var dirVector3 = new Vector3(signedDir.x, signedDir.y, 0);
            Indicator.rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), dirVector3);
        }
    }
}
