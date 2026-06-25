using System;

namespace Quantum
{
    using UnityEngine;
    using Unity.Mathematics;

    public class AnimationView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public AnimationContainer Container;
        public StateAnimationMap Map;
        public CurveRenderManager RenderManager;

        public override void OnUpdateView()
        {
            if (!PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin) ||
                !PredictedFrame.TryGet<SaberData>(EntityRef, out var saber))
                return;

            var roninState = PredictedFrame.FindAsset(ronin.CurrentState);
            var saberState = PredictedFrame.FindAsset(saber.CurrentState);

            if (!Map.TryGet(roninState, out var animID))
                return;

            if (!Container.TryGetHolder(animID, out var animHolder))
                return;
            
            var dirDependent = false;
            AnimationData baseAnimData = null;
                    
            foreach (var baseAnim in animHolder.Bases)
            {
                if (baseAnim.Info.IsSaberDirDependent)
                    dirDependent = true;

                if (!dirDependent)
                    return;
                
                if (saber.Direction.Id == baseAnim.Info.SaberDirection)
                    baseAnimData = baseAnim;
            }

            var container = new FrameContainer
            {
                Span = new int2(0, 0),
                Guide = Array.Empty<FrameCurveContainer>(),
                Persistent = Array.Empty<FrameCurveContainer>(),
                Transient = Array.Empty<FrameCurveContainer>()
            };
            
            if (baseAnimData != null)
            {
                foreach (var frame in baseAnimData.Frames)
                {
                    if (IsWithinSpan(ronin.StateContext.StateFrame, frame.Span))
                    {
                        container = frame;
                    }
                }
            }
            
            RenderManager.RenderFrame(container);
        }
        
        private bool IsWithinSpan(int value, int2 span)
        {
            return value >= span.x && value <= span.y;
        }
    }
}
