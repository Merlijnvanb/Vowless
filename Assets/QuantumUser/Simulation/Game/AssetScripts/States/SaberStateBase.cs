using System.Numerics;

namespace Quantum
{
    using Photon.Deterministic;
    
    public abstract unsafe class SaberStateBase : AssetObject
    {
        public SaberBlockType BlockType;
        
        public virtual void EnterState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame = 0;
        }

        public virtual void UpdateState(Frame frame, EntityRef entity)
        {
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            saber->StateFrame++;
        }

        public virtual AnimationID GetAnimationID(Frame frame, EntityRef entity)
        {
            return AnimationID.HoldingFwMid;
        }

        protected virtual SaberDirectionData PollDirection(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            
            var constants = frame.FindAsset(saber->Constants);
            var inputDir = player->InputLookDirectionVector;

            if (inputDir != FPVector2.Zero && inputDir.Magnitude > constants.InputMagnitudeThreshold)
            {
                var signedInput = new FPVector2(inputDir.X * ronin->FacingSign, inputDir.Y);
                saber->Direction = InputUtils.SnapToDirection(frame, signedInput, constants);
            }

            return saber->Direction;
        }
    }
}