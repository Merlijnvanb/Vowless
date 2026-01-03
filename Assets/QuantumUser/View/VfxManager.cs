namespace Quantum
{
    using UnityEngine;

    public class VfxManager : QuantumEntityViewComponent<IQuantumViewContext>
    {
        public GameObject DeflectEffectPrefab;
        
        public override void OnInitialize()
        {
            QuantumEvent.Subscribe<EventVfxDeflect>(listener: this, handler: OnDeflectEffect);
        }

        private void OnDeflectEffect(EventVfxDeflect data)
        {
            if (data.entity != EntityRef)
                return;

            Debug.Log("Fired deflect effect supposedly");
            
            var flatPoint = data.result.MidPoint.ToUnityVector2();
            var point = new Vector3(flatPoint.x, flatPoint.y, 0);
            
            var effect = Instantiate(DeflectEffectPrefab, transform);
            effect.transform.position = point;

            var attack = PredictedPreviousFrame.FindAsset(data.result.AttackState);
            var attackerFacingSign = PredictedPreviousFrame.Get<RoninData>(data.result.Attacker).FacingSign;
            
            var attackDir = attack.VisualDirection.ToUnityVector3().normalized;
            attackDir.x *= attackerFacingSign;
            attackDir.z *= attackerFacingSign;
            
            var rot = Quaternion.LookRotation(attackDir);
            effect.transform.rotation = rot;
        }
    }
}
