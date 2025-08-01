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
    }
}
