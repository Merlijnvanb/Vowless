namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class InputUtils
    {
        public static Input GetInput(Frame frame, EntityRef entity)
        {
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            return player->InputHistory[player->InputHeadIndex];
        }
        
        public static SaberDirectionData SnapToDirection(Frame frame, FPVector2 direction, SaberConstants constants)
        {
            var maxDot = FP.MinValue;
            var closest = new SaberDirectionData();

            foreach (var dir in constants.DirectionData)
            {
                var dot = FPVector2.Dot(direction, dir.Value.Vector.Normalized);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    closest = dir.Value;
                }
            }

            return closest;
        }
    }
}
