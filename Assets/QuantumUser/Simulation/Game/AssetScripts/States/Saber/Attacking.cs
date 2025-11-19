namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class Attacking : SaberStateBase
    {
        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame++;
            
            //saber->Direction = GetDirection(frame, entity);
        }
        
        public override AnimationID GetAnimationID(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var state = frame.FindAsset(ronin->CurrentState);
            var attack = state as AttackStateBase;

            return attack.AnimationID;
        }
    }
}
