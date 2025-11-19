namespace Quantum
{
    using Photon.Deterministic;
    
    public unsafe class Holding : SaberStateBase
    {
        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame++;
            
            saber->Direction = PollDirection(frame, entity);
        }

        public override AnimationID GetAnimationID(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);

            switch (saber->Direction.Id)
            {
                case SaberDirection.FwLow:
                    return AnimationID.HoldingFwLow;
                
                case SaberDirection.FwMid:
                    return AnimationID.HoldingFwMid;
                
                case SaberDirection.FwHigh:
                    return AnimationID.HoldingFwHigh;
                
                
                case SaberDirection.BwLow:
                    return AnimationID.HoldingBwLow;
                
                case SaberDirection.BwMid:
                    return AnimationID.HoldingBwMid;
                
                case SaberDirection.BwHigh:
                    return AnimationID.HoldingBwHigh;
                
                
                default:
                    return AnimationID.HoldingFwMid;
            }
        }
    }
}