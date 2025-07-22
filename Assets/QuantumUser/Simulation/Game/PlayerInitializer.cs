namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerInitializer : SystemSignalsOnly, ISignalOnPlayerAdded
    {
        public override void OnInit(Frame frame)
        {
            for (int i = 1; i < 3; i++)
            {
                var config = frame.FindAsset<GameConfig>(frame.RuntimeConfig.GameConfig);
                var roninEntity = frame.Create(frame.FindAsset(config.BaseRonin));
                
                var ronin = frame.Unsafe.GetPointer<RoninData>(roninEntity);
                var player = frame.Unsafe.GetPointer<PlayerData>(roninEntity);

                ronin->Position = i == 1
                    ? new FPVector2(-(config.StartDistance / 2), 0)
                    : new FPVector2(config.StartDistance / 2, 0);
                ronin->FacingSign = i == 1 ? 1 : -1;
                ronin->CurrentState = config.StartingState;
                ronin->StateFrame = 0;

                player->InputHeadIndex = -1;

                if (i == 1)
                    frame.Global->Ronin1 = roninEntity;
                else
                    frame.Global->Ronin2 = roninEntity;
            }
        }

        public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime)
        {
            if (player._index > 2)
                return;

            var config = frame.FindAsset<GameConfig>(frame.RuntimeConfig.GameConfig);
            var roninEntity = player._index == 1 ? frame.Global->Ronin1 : frame.Global->Ronin2;
            var playerData = frame.Unsafe.GetPointer<PlayerData>(roninEntity);
            var roninData = frame.Unsafe.GetPointer<RoninData>(roninEntity);
            var constants = frame.FindAsset<RoninConstants>(config.BaseConstants);

            playerData->PlayerRef = player;
            roninData->Constants = constants;
        }
    }
}
