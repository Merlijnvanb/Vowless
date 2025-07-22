namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class InputSystem : SystemMainThreadFilter<InputSystem.Filter>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            var player = filter.Player; ;
            
            player->InputHeadIndex = (player->InputHeadIndex + 1) % player->InputHistory.Length;

            if (!frame.Global->ParseInputs)
                player->InputHistory[player->InputHeadIndex] = new Input();
            else
                player->InputHistory[player->InputHeadIndex] = *frame.GetPlayerInput(player->PlayerRef);
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerData* Player;
            public RoninData* Ronin;
        }
    }
}
