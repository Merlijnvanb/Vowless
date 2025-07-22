namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;
    
    public abstract unsafe class RoninStateBase : AssetObject
    {
        public bool AlwaysCancelable = false;

        public virtual void EnterState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame = 0;
        }

        public virtual void UpdateState(Frame frame, EntityRef entity)
        {
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            ronin->StateFrame++;
        }

        protected virtual RoninStateBase GetNextState(Frame frame, EntityRef entity)
        {
            var player = frame.Unsafe.GetPointer<PlayerData>(entity);
            var input = InputUtils.GetInput(player);
            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var constants = frame.FindAsset<RoninConstants>(ronin->Constants);
            
            // if (input.Attack)
            //     // return attack
            // if (input.Block)
            //     // return block
            // if (input.Turn)
            //     // return turn
            if (input.MoveDir.X != 0)
                return constants.States.WalkState;

            return constants.States.IdleState;
        }
    }
}