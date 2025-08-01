namespace Quantum  
{  
    using Photon.Deterministic;  
  
    public unsafe class WalkState : RoninStateBase
    {
        public FP ForwardWalkSpeed;
        public FP BackwardWalkSpeed;

        public override void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
            
            ApplyWalk(frame, entity);
            
            if (AlwaysCancelable)
            {
                var nextState = GetNextState(frame, entity);
                if (nextState != this)
                    frame.Signals.OnSwitchRoninState(entity, nextState);
            }
        }

        private void ApplyWalk(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var input = InputUtils.GetInput(frame, entity);

            var signedDir = ronin->FacingSign * input.MoveDir.X;
            var moveSpeed = signedDir > 0 ? ForwardWalkSpeed : BackwardWalkSpeed;
            moveSpeed *= frame.DeltaTime;

            ronin->Position.X += moveSpeed * input.MoveDir.X;
        }
    }
}