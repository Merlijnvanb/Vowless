namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class AttackStateBase : RoninStateBase
    {
        [System.Serializable]
        public struct HitBoxData
        {
            public BoxRect Rect;
            public IntVector2 StartEndFrame;
        }
        
        public AttackHeight Height;
        public HitBoxData[] HitBoxes;
        public int ReceivedBlockStun;
        public FP DevotionGain;
        public bool TurnAround;
        public SaberDirection EndingDirection;
        
        public AnimationID AnimationID;

        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame = 0;
            ronin->HasHit = false;
            
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var saberConstants = frame.FindAsset(saber->Constants);

            if (saberConstants.DirectionData.TryGetValue(EndingDirection, out var newDir))
            {
                saber->Direction = newDir;
            }

            if (saber->CurrentState != saberConstants.States.Attacking)
            {
                frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Attacking);
            }
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var rConstants =  frame.FindAsset(ronin->Constants);
            ronin->StateFrame++;
            
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var saberConstants = frame.FindAsset(saber->Constants);

            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            
            var input = InputUtils.GetInput(frame, entity);
            if (input.Turn.WasPressed)
            {
                foreach (var window in TurnCancelWindows)
                {
                    if (ronin->StateFrame >= window.StartEndFrame.X && ronin->StateFrame <= window.StartEndFrame.Y)
                    {
                        var cost = window.CancelCost;

                        if (window.HasWhiffCost && !ronin->HasHit)
                        {
                            cost = window.WhiffCost;
                        }
                        
                        var signedInput = FPMath.SignZeroInt(player->InputMoveDirectionVector.X);

                        var nextState = rConstants.States.TurningState as RoninStateBase;
                        if (signedInput == ronin->FacingSign)
                            nextState = rConstants.States.TurningStateForward;
                        if (signedInput == -ronin->FacingSign)
                            nextState = rConstants.States.TurningStateBackward;

                        if (cost > ronin->Devotion)
                            break;
                        
                        Log.Debug(nextState.name);
                        
                        frame.Signals.OnDecreaseDevotion(entity, cost);
                        frame.Signals.OnSwitchRoninState(entity, nextState);
            
                        if (saber->CurrentState != saberConstants.States.Holding)
                        {
                            frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Holding);
                        }

                        return;
                    }
                }
            }

            if (input.Attack.WasPressed)
            {
                if (ronin->HasHit)
                {
                    if (TurnAround)
                    {
                        ronin->FacingSign *= -1;
                    }
                    
                    var nextState = GetNextState(frame, entity, out var newDir);
                    if (nextState is AttackStateBase)
                    {
                        var cost = FP._0;
                        
                        if (EndingDirection != newDir.Id)
                        {
                            cost = config.NonLinearAttackCost;
                        }
                        
                        if (cost <= ronin->Devotion)
                        {
                            frame.Signals.OnDecreaseDevotion(entity, cost);
                            frame.Signals.OnSwitchRoninState(entity, nextState);
                            frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Attacking);
                        }
                    }
                }
            }
            
            if (ronin->StateFrame > Duration)
            {
                if (TurnAround)
                {
                    ronin->FacingSign *= -1;
                }
                
                var nextState = GetNextState(frame, entity);
                frame.Signals.OnSwitchRoninState(entity, nextState);
            
                if (saber->CurrentState != saberConstants.States.Holding)
                {
                    frame.Signals.OnSwitchSaberState(entity, saberConstants.States.Holding);
                }
            }
        }

        // public virtual bool IsActive(Frame frame, EntityRef entity)
        // {
        //     var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
        //     
        //     return ronin->StateFrame > StartupFrames && ronin->StateFrame <= StartupFrames + ActiveFrames;
        // }
    }
}
