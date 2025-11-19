namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class TargetSystem : SystemMainThread, ISignalOnCalculateTarget
    {
        public override void Update(Frame frame)
        {
            var ronin1 = frame.Unsafe.GetPointer<RoninData>(frame.Global->Ronin1);
            var ronin2 = frame.Unsafe.GetPointer<RoninData>(frame.Global->Ronin2);

            var ronin1State = frame.FindAsset(ronin1->CurrentState);
            var ronin2State = frame.FindAsset(ronin2->CurrentState);

            if (ronin1->Position.X < ronin2->Position.X)
            {
                if (ronin1State.CanSwitchTarget) ronin1->TargetingSign = 1;
                if (ronin2State.CanSwitchTarget) ronin2->TargetingSign = -1;
            }

            if (ronin1->Position.X > ronin2->Position.X)
            {
                if (ronin1State.CanSwitchTarget) ronin1->TargetingSign = -1;
                if (ronin2State.CanSwitchTarget) ronin2->TargetingSign = 1;
            }
        }

        public void OnCalculateTarget(Frame f, EntityRef entity)
        {
            var otherEntity = entity == f.Global->Ronin1 ? f.Global->Ronin2 : f.Global->Ronin1;
            
            var ronin = f.Unsafe.GetPointer<RoninData>(entity);
            var otherRonin = f.Unsafe.GetPointer<RoninData>(otherEntity);

            if (ronin->Position.X < otherRonin->Position.X)
            {
                ronin->TargetingSign = 1;
            }
            
            if (ronin->Position.X > otherRonin->Position.X)
            {
                ronin->TargetingSign = -1;
            }
        }
    }
}
