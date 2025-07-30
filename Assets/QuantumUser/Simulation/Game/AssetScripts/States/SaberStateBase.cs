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

        protected virtual SaberDirectionData GetDirection(Frame frame, EntityRef entity)
        {
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);
            
            var constants = frame.FindAsset(saber->Constants);
            var input = InputUtils.GetInput(player);

            if (input.LookDir != FPVector2.Zero && input.LookDir.Magnitude > constants.InputMagnitudeThreshold)
            {
                var signedInput = new FPVector2(input.LookDir.X * ronin->FacingSign, input.LookDir.Y);
                saber->Direction = SnapToDirection(signedInput, constants);
            }

            return saber->Direction;
        }

        private SaberDirectionData SnapToDirection(FPVector2 direction, SaberConstants constants)
        {
            var maxDot = FP.MinValue;
            var closestId = SaberDirection.FwMid;
            FPVector2 closestVector = FPVector2.Zero;

            foreach (var dir in constants.Directions)
            {
                var dot = FPVector2.Dot(direction, dir.Value);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    closestId = dir.Key;
                    closestVector = dir.Value;
                }
            }

            return new SaberDirectionData()
            {
                Id = closestId,
                Vector = closestVector
            };
        }
    }
}