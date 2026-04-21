namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;
    
    public abstract unsafe class RoninStateBase : AssetObject
    {
        [System.Serializable]
        public struct TurnCancelWindow
        {
            public IntVector2 StartEndFrame;
            public FP CancelCost;

            public bool HasWhiffCost;
            public FP WhiffCost;
        }

        [System.Serializable]
        public struct MovementData
        {
            public IntVector2 StartEndFrame;
            public FPVector2 MoveVector;
        }

        [System.Serializable]
        public struct HurtBoxData
        {
            public bool UseBaseRect;
            public BoxRect Rect;
            public bool AlwaysActive;
            public IntVector2 StartEndFrame;
        }

        public int Duration;
        public bool AlwaysCancelable = false;
        public bool CanSwitchTarget = false;
        
        public TurnCancelWindow[] TurnCancelWindows;
        public MovementData[] Movements;

        public HurtBoxData[] HurtBoxes;

        public virtual AnimationID GetAnimationID(Frame frame, EntityRef entity)
        {
            return AnimationID.Idle;
        }

        public virtual void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateContext.StateFrame = 0;
        }

        public virtual void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateContext.StateFrame++;
            
            if (AlwaysCancelable)
            {
                var nextState = GetNextState(frame, entity);
                frame.Signals.OnSwitchRoninState(entity, nextState);
            }
        }

        protected virtual void ApplyMovement(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            
            foreach (var data in Movements)
            {
                if (ronin->StateContext.StateFrame >= data.StartEndFrame.X && ronin->StateContext.StateFrame < data.StartEndFrame.Y)
                {
                    ronin->Position += new FPVector2(
                        data.MoveVector.X * ronin->FacingSign * frame.DeltaTime, 
                        data.MoveVector.Y * frame.DeltaTime);
                }
            }
        }

        protected RoninStateBase GetNextState(Frame frame, EntityRef entity)
            => GetNextState(frame, entity, out _);

        protected virtual RoninStateBase GetNextState(Frame frame, EntityRef entity, out SaberDirectionData newDir)
        {
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var input = InputUtils.GetInput(frame, entity);

            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var rConstants = frame.FindAsset<RoninConstants>(ronin->Constants);

            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var sConstants = frame.FindAsset<SaberConstants>(saber->Constants);

            newDir = saber->Direction;

            if (input.Attack.WasPressed)
            {
                var dir = saber->Direction;

                if (player->InputLookDirectionVector != FPVector2.Zero)
                {
                    var signedInput = new FPVector2(player->InputLookDirectionVector.X * ronin->FacingSign, player->InputLookDirectionVector.Y);
                    dir = InputUtils.SnapToDirection(frame, signedInput, sConstants);
                }

                newDir = dir;

                var turned = ronin->TargetingSign != ronin->FacingSign;
                switch (dir.Id)
                {
                    case SaberDirection.FwHigh:
                        return turned ? rConstants.Attacks.TurnedForwardHigh : rConstants.Attacks.ForwardHigh;

                    case SaberDirection.FwMid:
                        return turned ? rConstants.Attacks.TurnedForwardMid : rConstants.Attacks.ForwardMid;

                    case SaberDirection.FwLow:
                        return turned ? rConstants.Attacks.TurnedForwardLow : rConstants.Attacks.ForwardLow;

                    case SaberDirection.BwHigh:
                        return turned ? rConstants.Attacks.TurnedBackwardHigh : rConstants.Attacks.BackwardHigh;

                    case SaberDirection.BwMid:
                        return turned ? rConstants.Attacks.TurnedBackwardMid : rConstants.Attacks.BackwardMid;

                    case SaberDirection.BwLow:
                        return turned ? rConstants.Attacks.TurnedBackwardLow : rConstants.Attacks.BackwardLow;
                }
            }

            if (input.Turn.WasPressed)
            {
                if ((int)player->InputMoveDirectionVector.X == ronin->FacingSign)
                    return rConstants.States.TurningStateForward;

                if ((int)player->InputMoveDirectionVector.X == -ronin->FacingSign)
                    return rConstants.States.TurningStateBackward;

                return rConstants.States.TurningState;
            }

            if (player->InputMoveDirectionVector.X != 0)
                return rConstants.States.WalkState;

            return rConstants.States.IdleState;
        }
    }
}