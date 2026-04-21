namespace Quantum
{
    using UnityEngine;

    public class SaberHitReactionView : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public Transform SaberTransform;
        public Transform BladeStartTransform;
        public Transform BladeEndTransform;

        private bool _pendingReaction;
        private Vector2 _impactMidPoint;

        public override void OnInitialize()
        {
            QuantumEvent.Subscribe<EventOnReceivedHit>(listener: this, handler: OnReceivedHit);
        }

        private void OnReceivedHit(EventOnReceivedHit e)
        {
            if (e.entity != EntityRef || !e.result.SaberHit) return;
            _pendingReaction = true;
            _impactMidPoint = e.result.MidPoint.ToUnityVector2();
        }

        // Runs after OnUpdateView so SaberAnimationView has already set the base transform.
        private void LateUpdate()
        {
            if (!_pendingReaction) return;
            _pendingReaction = false;

            var closestPoint = GetClosestPointOnBlade(_impactMidPoint);
            var offset = _impactMidPoint - closestPoint;
            var pos = SaberTransform.position;
            SaberTransform.position = new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z);
        }

        private Vector2 GetClosestPointOnBlade(Vector2 point)
        {
            var start = new Vector2(BladeStartTransform.position.x, BladeStartTransform.position.y);
            var end = new Vector2(BladeEndTransform.position.x, BladeEndTransform.position.y);
            var startToEnd = end - start;
            var t = Vector2.Dot(point - start, startToEnd) / startToEnd.sqrMagnitude;
            return start + startToEnd * Mathf.Clamp01(t);
        }
    }
}
