namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameSystem : SystemMainThread, ISignalOnRoninDeath
    {
        public override void OnInit(Frame frame)
        {
            var gameConfig = frame.FindAsset<GameConfig>(frame.RuntimeConfig.GameConfig);
            
            //Initialize global data
            SwitchGameState(frame, GameState.PreBattle, gameConfig.PreBattleTimeFrames);
            frame.Global->ParseInputs = false;
        }
        
        public override void Update(Frame frame)
        {
            switch (frame.Global->GameState)
            {
                case GameState.PreBattle:
                    if (frame.Global->GameTimer.HasStoppedThisFrame(frame))
                    {
                        SwitchGameState(frame, GameState.Battle);
                        frame.Global->ParseInputs = true;
                    }

                    break;
                
                case GameState.Battle:
                    break;
                
                case GameState.PostBattle:
                    if (frame.Global->GameTimer.HasStoppedThisFrame(frame))
                    {
                        ResetBattle(frame);
                    }
                    break;
            }
        }

        public void OnRoninDeath(Frame frame, EntityRef entity)
        {
            Log.Debug(entity + " ded");
            var gameConfig = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            
            SwitchGameState(frame, GameState.PostBattle, gameConfig.PostBattleTimeFrames);
            frame.Global->ParseInputs = false;
        }

        private void SwitchGameState(Frame frame, GameState state, int timerFrames = 0)
        {
            frame.Global->GameState = state;
            frame.Global->GameTimer = FrameTimer.FromFrames(frame, timerFrames);
            Log.Debug("GameState set to: " + state + ", timer set to: " + timerFrames + " frames");
        }

        private void ResetBattle(Frame frame)
        {
            PlayerInitializer.InitEntity(frame, frame.Global->Ronin1, 1);
            PlayerInitializer.InitEntity(frame, frame.Global->Ronin2, 2);
            
            var gameConfig = frame.FindAsset<GameConfig>(frame.RuntimeConfig.GameConfig);
            SwitchGameState(frame, GameState.PreBattle, gameConfig.PreBattleTimeFrames);
        }
    }
}
