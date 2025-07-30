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
    }
}
