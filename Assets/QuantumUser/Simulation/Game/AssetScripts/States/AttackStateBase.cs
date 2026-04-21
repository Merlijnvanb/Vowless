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

        public FPVector3 VisualDirection;
        public AnimationID AnimationID;
        public AnimationID RoninAnimationID;

        public override AnimationID GetAnimationID(Frame frame, EntityRef entity)
        {
            return RoninAnimationID;
        }

        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateContext.StateFrame = 0;
            ronin->StateContext.HasHit = false;

            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var saberConstants = frame.FindAsset(saber->Constants);
            saber->AttackAnimationID = AnimationID;

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
            ronin->StateContext.StateFrame++;
            
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var saberConstants = frame.FindAsset(saber->Constants);

            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            
            var input = InputUtils.GetInput(frame, entity);
            if (input.Turn.WasPressed)
            {
                foreach (var window in TurnCancelWindows)
                {
                    if (ronin->StateContext.StateFrame >= window.StartEndFrame.X && ronin->StateContext.StateFrame <= window.StartEndFrame.Y)
                    {
                        var cost = window.CancelCost;

                        if (window.HasWhiffCost && !ronin->StateContext.HasHit)
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
                if (ronin->StateContext.HasHit)
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
            
            if (ronin->StateContext.StateFrame > Duration)
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
        //     return ronin->StateContext.StateFrame > StartupFrames && ronin->StateContext.StateFrame <= StartupFrames + ActiveFrames;
        // }
    }
}
