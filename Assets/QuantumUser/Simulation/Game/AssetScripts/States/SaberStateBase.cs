namespace Quantum
{
    using Photon.Deterministic;
    
    public abstract unsafe class SaberStateBase : AssetObject
    {
        public virtual void EnterState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame = 0;
        }

        public virtual void UpdateState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame++;
        }
    }
}