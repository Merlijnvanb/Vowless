using UnityEngine;

namespace Quantum
{
    public unsafe class TargetGroupManager : QuantumSceneViewComponent
    {
        public Transform P1Target;
        public Transform P2Target;

        void Update()
        {
            if (PredictedPreviousFrame == null)
                return;
            
            if (PredictedPreviousFrame.TryGet<RoninData>(PredictedPreviousFrame.Global->Ronin1, out var ronin1))
                SetPosition(ronin1, P1Target);
            
            if (PredictedPreviousFrame.TryGet<RoninData>(PredictedPreviousFrame.Global->Ronin2, out var ronin2))
                SetPosition(ronin2, P2Target);
        }

        private void SetPosition(RoninData ronin, Transform target)
        {
            var vec2pos = ronin.Position.ToUnityVector2();
            var vectorPos = new Vector3(vec2pos.x, vec2pos.y, 0);
            
            target.position = vectorPos;
        }
    }
}
