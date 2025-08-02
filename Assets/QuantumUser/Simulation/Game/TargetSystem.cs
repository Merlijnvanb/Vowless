namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class TargetSystem : SystemMainThread
    {
        public override void Update(Frame frame)
        {
            var ronin1 = frame.Unsafe.GetPointer<RoninData>(frame.Global->Ronin1);
            var ronin2 = frame.Unsafe.GetPointer<RoninData>(frame.Global->Ronin2);

            if (ronin1->Position.X < ronin2->Position.X)
            {
                ronin1->TargetingSign = 1;
                ronin2->TargetingSign = -1;
            }

            if (ronin1->Position.X > ronin2->Position.X)
            {
                ronin1->TargetingSign = -1;
                ronin2->TargetingSign = 1;
            }
        }
    }
}
