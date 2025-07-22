namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class InputUtils
    {
        public static Input GetInput(PlayerData* player)
        {
            return player->InputHistory[player->InputHeadIndex];
        }
    }
}
