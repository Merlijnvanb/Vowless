namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerInitializer : SystemSignalsOnly, ISignalOnPlayerAdded
    {
        public override void OnInit(Frame frame)
        {
            for (int i = 1; i <= 2; i++)
            {
                var config = frame.FindAsset<GameConfig>(frame.RuntimeConfig.GameConfig);
                var roninEntity = frame.Create(frame.FindAsset(config.BaseRonin));
                
                var player = frame.Unsafe.GetPointer<PlayerData>(roninEntity);
                
                player->InputHeadIndex = -1;
                
                
                var ronin = frame.Unsafe.GetPointer<RoninData>(roninEntity);

                ronin->Position = i == 1
                    ? new FPVector2(-(config.StartDistance / 2), 0)
                    : new FPVector2(config.StartDistance / 2, 0);
                ronin->FacingSign = i == 1 ? 1 : -1;
                ronin->CurrentState = config.StartingRoninState;
                ronin->StateFrame = 0;
                
                
                var saber = frame.Unsafe.GetPointer<SaberData>(roninEntity);

                var directionData = new SaberDirectionData()
                {
                    Id = SaberDirection.FwMid,
                    Vector = new FPVector2(1, 0)
                };
                saber->Direction = directionData;
                saber->CurrentState = config.StartingSaberState;
                saber->StateFrame = 0;
                

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
            
            playerData->PlayerRef = player;
            
            
            var roninData = frame.Unsafe.GetPointer<RoninData>(roninEntity);
            var roninConstants = frame.FindAsset<RoninConstants>(config.BaseRoninConstants);

            roninData->Constants = roninConstants;
            
            
            var saberData = frame.Unsafe.GetPointer<SaberData>(roninEntity);
            var saberConstants = frame.FindAsset<SaberConstants>(config.BaseSaberConstants);
            saberConstants.InitData();
            
            saberData->Constants = saberConstants;
        }
    }
}
