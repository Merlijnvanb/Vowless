using Photon.Deterministic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quantum
{
    public class GameInput : MonoBehaviour
    {
        public int PlayerIndex;
        
        private Quantum.Input currentInput;
        
        void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        private void PollInput(CallbackPollInput callback)
        {
            if (callback.PlayerSlot != PlayerIndex)
                return;
            
            Quantum.Input i = new Quantum.Input();
            
            //Debug.Log(callback.PlayerSlot);
            i = GetInputs(i, false);

            callback.SetInput(i, DeterministicInputFlags.Repeatable);
        }
        
        private Quantum.Input GetInputs(Quantum.Input i, bool isMouse)
        {
            i.IsMouseInput = isMouse;

            i.MoveDir = currentInput.MoveDir;
            i.LookDir = currentInput.LookDir;

            i.Attack = currentInput.Attack;
            i.Block = currentInput.Block;
            i.Turn = currentInput.Turn;
            
            return i;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            currentInput.MoveDir = context.action.ReadValue<Vector2>().ToFPVector2();
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            currentInput.LookDir = context.action.ReadValue<Vector2>().ToFPVector2();
        }
        
        public void OnAttack(InputAction.CallbackContext context)
        {
            currentInput.Attack = context.action.IsPressed();
        }
        
        public void OnBlock(InputAction.CallbackContext context)
        {
            currentInput.Block = context.action.IsPressed();
        }
        
        public void OnTurn(InputAction.CallbackContext context)
        {
            currentInput.Turn = context.action.IsPressed();
        }
    }
}