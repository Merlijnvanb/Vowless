namespace Quantum
{
    using System;
    using System.Linq;
    using UnityEngine;
    using Unity.Mathematics;
    using System.Collections.Generic;

    public class AnimationView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public AnimationContainer Container;
        public StateAnimationMap Map;
        public CurveRenderManager RenderManager;
        
        private readonly Dictionary<CurveID, FrameCurveContainer> _persistent = new();
        private readonly List<FrameCurveContainer> _transient = new();
        private readonly List<AnimationData> _partials = new();

        public override void OnUpdateView()
        {
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (!PredictedFrame.TryGet<RoninData>(EntityRef, out var ronin) ||
                !PredictedFrame.TryGet<SaberData>(EntityRef, out var saber))
                return;

            var roninState = PredictedFrame.FindAsset(ronin.CurrentState);
            var saberState = PredictedFrame.FindAsset(saber.CurrentState);
            
            _persistent.Clear();
            _transient.Clear();

            RenderContainer finalContainer = new RenderContainer
            {
                Persistent = _persistent,
                Transient = _transient
                //Guide = new List<FrameCurveContainer>()
            };
            
            if (!Map.TryGet(roninState, out var animID))
            {
                Debug.LogWarning("Couldn't get AnimID from map with state: " + roninState.name);
                RenderManager.RenderFrame(finalContainer);
                return;
            }

            if (!Container.TryGetHolder(animID, out var animHolder))
            {
                Debug.LogWarning("Couldn't get animHolder from animID: " + animID);
                RenderManager.RenderFrame(finalContainer);
                return;
            }

            var stateFrame = ronin.StateContext.StateFrame;
            
            if (TryGetBase(animHolder, saber, out var baseData))
            {
                var frameIndex = baseData.Info.IsLoop ? stateFrame % baseData.Info.Duration : stateFrame;
                
                foreach (var frame in baseData.Frames)
                {
                    if (!IsWithinSpan(frameIndex, frame.Span))
                        continue;

                    foreach (var curves in frame.Persistent)
                    {
                        finalContainer.Persistent[curves.ID] = curves;
                    }

                    finalContainer.Transient.AddRange(frame.Transient);
                }
            }
            
            if (TryGetPartials(animHolder, saber))
            {
                foreach (var data in _partials)
                {
                    var frameIndex = data.Info.IsLoop ? stateFrame % data.Info.Duration : stateFrame;

                    foreach (var frame in data.Frames)
                    {
                        if (!IsWithinSpan(frameIndex, frame.Span))
                            continue;

                        foreach (var curves in frame.Persistent)
                        {
                            if (data.Info.PartialCurves.Contains(curves.ID))
                                finalContainer.Persistent[curves.ID] = curves;
                        }

                        finalContainer.Transient.AddRange(frame.Transient);
                    }
                }
            }

            RenderManager.RenderFrame(finalContainer);
        }

        private bool TryGetBase(AnimationContainer.CategorizedHolder holder, SaberData saber, out AnimationData data)
        {
            data = null;

            if (holder.Bases == null || holder.Bases.Length == 0 || holder.Bases[0] == null)
                return false;
            
            if (holder.Bases[0].Info.IsSaberDirDependent) // this shit sucks ass
            {
                foreach (var b in holder.Bases)
                {
                    if (saber.Direction.Id != b.Info.SaberDirection)
                        continue;

                    data = b;
                    return true;
                }
            }
            else
            {
                data = holder.Bases[0];
                return true;
            }

            return false;
        }

        private bool TryGetPartials(AnimationContainer.CategorizedHolder holder, SaberData saber)
        {
            _partials.Clear();

            if (holder.Partials == null || holder.Partials.Length == 0 || holder.Partials[0] == null)
                return false;

            foreach (var p in holder.Partials)
            {
                if (saber.Direction.Id != p.Info.SaberDirection && holder.Partials[0].Info.IsSaberDirDependent)
                    continue;

                _partials.Add(p);
            }

            return _partials.Count > 0;
        }
        
        private bool IsWithinSpan(int value, int2 span)
        {
            return value >= span.x && value <= span.y;
        }
    }
}
