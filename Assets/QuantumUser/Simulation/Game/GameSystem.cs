namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameSystem : SystemMainThread
    {
        public override void OnInit(Frame frame)
        {
            var gameConfig = frame.FindAsset<GameConfig>(frame.RuntimeConfig.GameConfig);
            
            //Initialize global data
            SwitchGameState(frame, GameState.PREBATTLE);
            frame.Global->GameTimer = FrameTimer.FromFrames(frame, gameConfig.PreBattleTimeFrames);
            frame.Global->ParseInputs = true;
        }
        
        public override void Update(Frame frame)
        {
            switch (frame.Global->GameState)
            {
                case GameState.PREBATTLE:
                    if (frame.Global->GameTimer.HasStoppedThisFrame(frame))
                    {
                        SwitchGameState(frame, GameState.BATTLE);
                    }

                    break;
                
                case GameState.BATTLE:
                    break;
                
                case GameState.POSTBATTLE:
                    break;
            }
        }

        private void SwitchGameState(Frame frame, GameState state)
        {
            frame.Global->GameState = state;
            Log.Debug("GameState set to: " + state);
        }
    }
}
