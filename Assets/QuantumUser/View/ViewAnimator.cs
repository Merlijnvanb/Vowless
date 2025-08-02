namespace Quantum
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ViewAnimator : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public MeshFilter HiltMeshFilter;
        public MeshFilter BladeMeshFilter;
        public Transform HiltTransform;
        
        public AnimDataHolder AnimationHolder;

        private Dictionary<AnimationID, AnimationData> saberAnimations;
        private Dictionary<AnimationID, AnimationData> roninAnimations;
        private Dictionary<AnimationID, AnimationData> attackAnimations;

        private AnimationData currentSaberAnim;

        public override void OnInitialize()
        {
            saberAnimations = new Dictionary<AnimationID, AnimationData>();
            roninAnimations = new Dictionary<AnimationID, AnimationData>();
            attackAnimations = new Dictionary<AnimationID, AnimationData>();
            
            foreach (var anim in AnimationHolder.SaberAnimations)
            {
                saberAnimations.Add(anim.ID, anim);
            }

            foreach (var anim in AnimationHolder.RoninAnimations)
            {
                roninAnimations.Add(anim.ID, anim);
            }

            foreach (var anim in AnimationHolder.AttackAnimations)
            {
                attackAnimations.Add(anim.ID, anim);
            }
        }

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin)) return;
            if (!PredictedFrame.TryGet<SaberData>(EntityRef, out var saber)) return;

            saberAnimations.TryGetValue(GetSaberAnimationID(ronin, saber), out var nextSaberAnim);
            if (nextSaberAnim != currentSaberAnim)
            {
                currentSaberAnim = nextSaberAnim;
            }
            
            UpdateAnimations(ronin, saber);
        }

        private void UpdateAnimations(RoninData ronin, SaberData saber)
        {
            UpdateSaberAnimation(saber);
        }

        private void UpdateSaberAnimation(SaberData saber)
        {
            var frameIndex = 0;
            var accumulatedFrames = 0;
            var isFrameSet = false;

            for (int i = 0; i < currentSaberAnim.Frames.Length; i++)
            {
                var frameLength = currentSaberAnim.Frames[i].ActiveLength;

                if (saber.StateFrame < accumulatedFrames + frameLength)
                {
                    frameIndex = i;
                    isFrameSet = true;
                    break;
                }

                accumulatedFrames += frameLength;
            }

            if (!isFrameSet)
            {
                frameIndex = currentSaberAnim.Loop ? 0 : currentSaberAnim.Frames.Length - 1;
            }

            var frame = currentSaberAnim.Frames[frameIndex];
            SetMeshes(frame.HiltMesh, frame.BladeMesh, frame.HiltTransform);
        }


        private void SetMeshes(Mesh hiltMesh, Mesh bladeMesh, Transform hiltTransform)
        {
            HiltMeshFilter.sharedMesh = hiltMesh;
            BladeMeshFilter.sharedMesh = bladeMesh;
            HiltTransform.SetLocalPositionAndRotation(hiltTransform.localPosition, hiltTransform.localRotation);
        }

        private AnimationID GetSaberAnimationID(RoninData ronin, SaberData saber)
        {
            var rConstants = PredictedFrame.FindAsset(ronin.Constants);
            var sConstants = PredictedFrame.FindAsset(saber.Constants);

            if (saber.CurrentState == sConstants.States.Holding)
            {
                switch (saber.Direction.Id)
                {
                    case (SaberDirection.FwHigh):
                        return AnimationID.HoldingFwHigh;
                    
                    case (SaberDirection.FwMid):
                        return AnimationID.HoldingFwMid;
                    
                    case (SaberDirection.FwLow):
                        return AnimationID.HoldingFwLow;
                    
                    case (SaberDirection.BwHigh):
                        return AnimationID.HoldingBwHigh;
                    
                    case (SaberDirection.BwMid):
                        return AnimationID.HoldingBwMid;
                    
                    case (SaberDirection.BwLow):
                        return AnimationID.HoldingBwLow;
                }
            }

            if (saber.CurrentState == sConstants.States.Attacking)
            {
                switch (saber.Direction.Id)
                {
                    case (SaberDirection.FwHigh):
                        return AnimationID.AttackFwHigh;
                    
                    case (SaberDirection.FwMid):
                        return AnimationID.AttackFwMid;
                    
                    case (SaberDirection.FwLow):
                        return AnimationID.AttackFwLow;
                    
                    case (SaberDirection.BwHigh):
                        return AnimationID.AttackBwHigh;
                    
                    case (SaberDirection.BwMid):
                        return AnimationID.AttackBwMid;
                    
                    case (SaberDirection.BwLow):
                        return AnimationID.AttackBwLow;
                }
            }

            return AnimationID.HoldingFwMid;
        }
    }
}
