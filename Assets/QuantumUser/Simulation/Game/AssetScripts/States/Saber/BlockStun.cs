namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class BlockStun : SaberStateBase
    {
        public override void EnterState(Frame frame, EntityRef entity)
        {
            
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var constants = frame.FindAsset(saber->Constants);
            saber->StateFrame--;

            if (saber->StateFrame == 0)
            {
                frame.Signals.OnSwitchSaberState(entity, constants.States.Holding);
            }
        }
        
        public override AnimationID GetAnimationID(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);

            switch (saber->Direction.Id)
            {
                case SaberDirection.FwLow:
                    return AnimationID.BlockStunFwLow;
                
                case SaberDirection.FwMid:
                    return AnimationID.BlockStunFwMid;
                
                case SaberDirection.FwHigh:
                    return AnimationID.BlockStunFwHigh;
                
                
                case SaberDirection.BwLow:
                    return AnimationID.BlockStunBwLow;
                
                case SaberDirection.BwMid:
                    return AnimationID.BlockStunBwMid;
                
                case SaberDirection.BwHigh:
                    return AnimationID.BlockStunBwHigh;
                
                
                default:
                    return AnimationID.BlockStunFwMid;
            }
        }
    }
}
