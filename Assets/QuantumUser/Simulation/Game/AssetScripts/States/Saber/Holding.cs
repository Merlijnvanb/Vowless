namespace Quantum
{
    using Photon.Deterministic;
    
    public unsafe class Holding : SaberStateBase
    {
        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateContext.StateFrame++;
            
            saber->Direction = PollDirection(frame, entity);
        }
    }
}