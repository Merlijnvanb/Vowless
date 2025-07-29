namespace Quantum
{
    using Photon.Deterministic;
    
    public unsafe class Holding : SaberStateBase
    {
        public override void EnterState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame = 0;
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame++;
            
            saber->Direction = GetDirection(frame, entity);
        }
    }
}