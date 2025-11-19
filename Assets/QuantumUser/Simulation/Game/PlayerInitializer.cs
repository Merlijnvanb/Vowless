namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerInitializer : SystemSignalsOnly, ISignalOnPlayerAdded
    {
        public override void OnInit(Frame frame)
        {
            var config = frame.FindAsset<GameConfig>(frame.RuntimeConfig.GameConfig);
            
            for (int i = 1; i <= 2; i++)
            {
                var roninEntity = frame.Create(frame.FindAsset(config.BaseRonin));
                
                var player = frame.Unsafe.GetPointer<PlayerData>(roninEntity);
                
                player->InputHeadIndex = -1;
                
                
                var ronin = frame.Unsafe.GetPointer<RoninData>(roninEntity);
                var roninConstants = frame.FindAsset<RoninConstants>(config.BaseRoninConstants);

                ronin->Constants = roninConstants;
                ronin->Devotion = config.BaseDevotion;
                ronin->Position = i == 1
                    ? new FPVector2(-(config.StartDistance / 2), 0)
                    : new FPVector2(config.StartDistance / 2, 0);
                ronin->Velocity = FPVector2.Zero;
                ronin->TargetingSign = i == 1 ? 1 : -1;
                ronin->FacingSign = i == 1 ? 1 : -1;
                ronin->HitBoxes = frame.AllocateList<HitBox>();
                ronin->HurtBoxes = frame.AllocateList<HurtBox>();
                ronin->IgnoreCollision = false;
                ronin->CurrentState = config.StartingRoninState;
                ronin->StateFrame = 0;
                ronin->HasHit = false;
                
                
                var saber = frame.Unsafe.GetPointer<SaberData>(roninEntity);
                var saberConstants = frame.FindAsset<SaberConstants>(config.BaseSaberConstants);
                saberConstants.InitData(frame);
            
                saber->Constants = saberConstants;
                
                // setup base direction
                var directionEditorData = config.BaseSaberDirection;
                var directionBoxes = frame.AllocateList<BoxRect>();

                foreach (var box in directionEditorData.Boxes)
                {
                    directionBoxes.Add(box);
                }

                var directionData = new SaberDirectionData()
                {
                    Id = directionEditorData.Id,
                    Vector = directionEditorData.Vector,
                    Boxes = directionBoxes
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
            
            var entity = player._index == 1 ? frame.Global->Ronin1 : frame.Global->Ronin2;
            var playerData = frame.Unsafe.GetPointer<PlayerData>(entity);
            
            playerData->PlayerRef = player;
        }
    }
}
