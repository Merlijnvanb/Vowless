namespace Quantum
{
    using UnityEngine;

    public class SaberAnimationView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public MeshFilter HiltMeshFilter;
        public MeshFilter BladeMeshFilter;
        public Transform SaberTransform;
        //public Transform PoleLTransform;
        //public Transform PoleRTransform;

        public Mesh BaseHiltMesh;
        public Mesh BaseBladeMesh;

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<SaberData>(EntityRef, out var saber)) return;
            if (!PredictedFrame.TryFindAsset(saber.Constants, out var constants)) return;
            if (!constants.SaberAnimations.TryGetValue(saber.CurrentAnimationID, out var animation)) return;

            var frame = animation.Frames[saber.CurrentAnimationFrameIndex];

            SaberTransform.SetLocalPositionAndRotation(
                frame.Position.ToUnityVector3(),
                Quaternion.Euler(frame.Rotation.ToUnityVector3()));

            //PoleLTransform.localPosition = frame.PoleLPosition.ToUnityVector3();
            //PoleRTransform.localPosition = frame.PoleRPosition.ToUnityVector3();

            HiltMeshFilter.mesh = frame.HiltMesh != null ? frame.HiltMesh : BaseHiltMesh;
            BladeMeshFilter.mesh = frame.BladeMesh != null ? frame.BladeMesh : BaseBladeMesh;
        }
    }
}
