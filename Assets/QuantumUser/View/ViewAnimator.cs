namespace Quantum
{
    using UnityEngine;
    using System.Collections.Generic;

    public enum AnimType
    {
        Saber,
        Ronin
    }

    public class ViewAnimator : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public MeshFilter HiltMeshFilter;
        public MeshFilter BladeMeshFilter;
        public Transform SaberTransform;
        public Transform PoleLTransform;
        public Transform PoleRTransform;
        
        public Transform BladeStartTransform;
        public Transform BladeEndTransform;
        
        public Mesh BaseHiltMesh;
        public Mesh BaseBladeMesh;

        private AnimationID previousAnimationID;
        private int previousSaberFrameIndex;

        public override void OnInitialize()
        {
            QuantumEvent.Subscribe<EventOnHit>(listener: this, handler: OnHit);
            QuantumEvent.Subscribe<EventOnReceivedHit>(listener: this, handler: OnReceivedHit);

            previousAnimationID = 0;
            previousSaberFrameIndex = -1;
        }

        private void OnHit(EventOnHit e)
        {
            if (e.entity != EntityRef)
                return;
            
            
        }

        private void OnReceivedHit(EventOnReceivedHit e)
        {
            if (e.entity != EntityRef || !e.result.SaberHit)
                return;
            
            var midPoint = e.result.MidPoint.ToUnityVector2();
            var closestPointOnSaber = GetClosestPointOnSaber(midPoint);
            SaberToPoint(closestPointOnSaber, midPoint, out var newPos, out var newRot);
            
            SetSaberWorld(newPos, newRot);
        }

        private void SaberToPoint(Vector2 saberPoint, Vector2 midPoint, out Vector3 newPos, out Quaternion newRot)
        {
            var difference = midPoint - saberPoint;
            var saberPos = SaberTransform.position;
            var newPoint = new Vector2(saberPos.x, saberPos.y) + difference;
            
            newPos = new Vector3(newPoint.x, newPoint.y, SaberTransform.position.z);
            newRot = SaberTransform.rotation;
        }

        private Vector2 GetClosestPointOnSaber(Vector2 point)
        {
            var start = new Vector2(BladeStartTransform.position.x, BladeStartTransform.position.y);
            var end = new Vector2(BladeEndTransform.position.x, BladeEndTransform.position.y);

            var startToPoint = point - start;
            var startToEnd = end - start;
            
            var startToEndSquared = startToEnd.sqrMagnitude;
            var dotProduct = Vector2.Dot(startToPoint, startToEnd);
            var distance = dotProduct / startToEndSquared;

            if (distance < 0)
                return start;
            
            if (distance > startToEndSquared)
                return end;
            
            return start + startToEnd * distance;
        }

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin)) return;
            if (!PredictedFrame.TryGet<SaberData>(EntityRef, out var saber)) return;
            
            UpdateAnimations(ronin, saber);
        }
        
        private void UpdateAnimations(RoninData ronin, SaberData saber)
        {
            UpdateSaberAnimation(saber);
        }
        
        private void UpdateSaberAnimation(SaberData saber)
        {
            if (!PredictedFrame.TryFindAsset(saber.Constants, out var constants)) return;

            if (!constants.SaberAnimations.TryGetValue(saber.CurrentAnimationID, out var animation)) return;
            var frame = animation.Frames[saber.CurrentAnimationFrameIndex];
            
            // this fucking sucks holy shit
            if (previousAnimationID == saber.CurrentAnimationID &&
                previousSaberFrameIndex == saber.CurrentAnimationFrameIndex)
                return;
            
            previousAnimationID = saber.CurrentAnimationID;
            previousSaberFrameIndex = saber.CurrentAnimationFrameIndex;

            var pos = frame.Position.ToUnityVector3();
            var rotEuler = frame.Rotation.ToUnityVector3();
            var rot = Quaternion.Euler(rotEuler);
            
            SetSaberLocal(pos, rot);

            var poleL = frame.PoleLPosition.ToUnityVector3();
            var poleR = frame.PoleRPosition.ToUnityVector3();
            
            PoleLTransform.localPosition = poleL;
            PoleRTransform.localPosition = poleR;
        }
        
        
        private void UpdateRoninAnimation(RoninData ronin)
        {
            
        }
        
        private void SetSaberLocal(Vector3 pos, Quaternion rot)
        {
            SaberTransform.SetLocalPositionAndRotation(pos, rot);
        }

        private void SetSaberWorld(Vector3 pos, Quaternion rot)
        {
            SaberTransform.SetPositionAndRotation(pos, rot);
        }
    }
}
