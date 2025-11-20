namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class InputSystem : SystemMainThreadFilter<InputSystem.Filter>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            UpdateInputIndex(frame, ref filter);
            UpdateDirectionVector(frame, ref filter);
        }

        private void UpdateInputIndex(Frame frame, ref Filter filter)
        {
            var player = filter.Player;
            
            player->InputHeadIndex = (player->InputHeadIndex + 1) % player->InputHistory.Length;

            if (!frame.Global->ParseInputs || !player->PlayerRef.IsValid)
                player->InputHistory[player->InputHeadIndex] = new Input();
            else
                player->InputHistory[player->InputHeadIndex] = *frame.GetPlayerInput(player->PlayerRef);
        }

        private void UpdateDirectionVector(Frame frame, ref Filter filter)
        {
            var player = filter.Player;

            player->InputMoveDirectionVector = DigitalizeMoveInput(frame, ref filter);
            player->InputLookDirectionVector = ApplyDirection(frame, ref filter);
        }
        
        private FPVector2 ApplyMouseDirection(Frame frame, ref Filter filter)
        {
            var player = filter.Player;
            var input = InputUtils.GetInput(frame, filter.Entity);

            var dirDelta = input.LookDir;
            var currentDir = player->InputLookDirectionVector;
            
            return currentDir + dirDelta;
        }

        private FPVector2 DigitalizeMoveInput(Frame frame, ref Filter filter)
        {
            var input = InputUtils.GetInput(frame, filter.Entity);
            var config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            
            var xDir = input.MoveDir.X;
            var yDir = input.MoveDir.Y;

            var digitalX = FP._0;
            var digitalY = FP._0;

            if (xDir > config.AnalogMoveDeadZone)
                digitalX += 1;
            
            if (xDir < -config.AnalogMoveDeadZone)
                digitalX -= 1;
            
            
            if (yDir > config.AnalogMoveDeadZone)
                digitalY += 1;
            
            if (yDir < -config.AnalogMoveDeadZone)
                digitalY -= 1;
            
            return new FPVector2(digitalX, digitalY);
        }
        
        private FPVector2 ApplyDirection(Frame frame, ref Filter filter)
        {
            var input = InputUtils.GetInput(frame, filter.Entity);
            
            return input.LookDir;
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
