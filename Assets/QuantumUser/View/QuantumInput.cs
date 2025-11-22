using UnityEngine;
using Photon.Deterministic;
using UnityEngine.InputSystem;

namespace Quantum
{
    public class QuantumInput : MonoBehaviour
    {
        public PlayerInput P1;
        public PlayerInput P2;
        
        void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        private void PollInput(CallbackPollInput callback)
        {
            Quantum.Input i = new Quantum.Input();
            
            //Debug.Log(callback.PlayerSlot);
            

            callback.SetInput(i, DeterministicInputFlags.Repeatable);
        }

        public void OnJoin(PlayerInput input)
        {
            P1 = input;
        }
    }
}
