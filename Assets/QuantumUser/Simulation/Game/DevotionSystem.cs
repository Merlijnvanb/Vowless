namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class DevotionSystem : SystemMainThreadFilter<DevotionSystem.Filter>, ISignalOnIncreaseDevotion, ISignalOnDecreaseDevotion
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            ClampDevotion(frame, filter.Entity);
        }
        
        public void OnIncreaseDevotion(Frame frame, EntityRef entity, FP amount)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->Devotion += amount;
            ClampDevotion(frame, entity);
        }

        public void OnDecreaseDevotion(Frame frame, EntityRef entity, FP amount)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->Devotion -= amount;
            ClampDevotion(frame, entity);
        }

        private void ClampDevotion(Frame frame, EntityRef entity)
        {
            var config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);

            var max = config.DevotionMax;
            ronin->Devotion = FPMath.Clamp(ronin->Devotion, 0, max);
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerData* Player;
            public RoninData* Ronin;
            public SaberData* Saber;
        }
    }
}
