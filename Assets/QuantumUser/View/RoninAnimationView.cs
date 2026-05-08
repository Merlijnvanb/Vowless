namespace Quantum
{
    using UnityEngine;

    public class RoninAnimationView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        // public MeshFilter BodyMeshFilter;
        // public Mesh BaseBodyMesh;
        //
        // public override void OnUpdateView()
        // {
        //     if (!PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin)) return;
        //     if (!PredictedFrame.TryFindAsset(ronin.Constants, out var constants)) return;
        //
        //     if (!constants.RoninAnimations.TryGetValue(ronin.CurrentRoninAnimationID, out var animation))
        //     {
        //         BodyMeshFilter.mesh = BaseBodyMesh;
        //         return;
        //     }
        //
        //     var frame = animation.Frames[ronin.CurrentRoninAnimationFrameIndex];
        //     BodyMeshFilter.mesh = frame.BodyMesh != null ? frame.BodyMesh : BaseBodyMesh;
        // }
    }
}
