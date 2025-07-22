namespace Quantum  
{  
    using Photon.Deterministic;  
  
    public unsafe class WalkState : RoninStateBase
    {
        public FP ForwardWalkSpeed;
        public FP BackwardWalkSpeed;
        
        public override void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);

            ronin->StateFrame = 0;
        }

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
            
            ApplyMovement(frame, entity);
            
            var nextState = GetNextState(frame, entity);
            if (nextState != this)
                frame.Signals.OnSwitchRoninState(entity, nextState);
        }

        private void ApplyMovement(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var input = InputUtils.GetInput(player);

            var signedDir = ronin->FacingSign * input.MoveDir.X;
            var moveSpeed = signedDir > 0 ? ForwardWalkSpeed : BackwardWalkSpeed;

            ronin->Position.X += moveSpeed * input.MoveDir.X;
        }
    }
}