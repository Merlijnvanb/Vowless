using UnityEngine;
using Photon.Deterministic;

namespace Quantum
{
    public class QuantumInput : MonoBehaviour
    {
        public GameInput KbmInput;
        public GameInput PadInput;
        
        public int KbmIndex = 0;
        public int PadIndex = 1;
        
        void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                var kbm = KbmIndex;
                var pad = PadIndex;

                KbmIndex = pad;
                PadIndex = kbm;
            }
        }

        private void PollInput(CallbackPollInput callback)
        {
            Quantum.Input i = new Quantum.Input();
            
            //Debug.Log(callback.PlayerSlot);

            if (callback.PlayerSlot == KbmIndex)
                i = KbmInput.GetInputs(i, false);
            
            else if (callback.PlayerSlot == PadIndex)
                i = PadInput.GetInputs(i, false);

            callback.SetInput(i, DeterministicInputFlags.Repeatable);
        }
        
        private void DebugVectors(Quantum.Input i, int playerIndex)
        {
            Debug.Log("MoveDir from PlayerIndex " + playerIndex + ": " + i.MoveDir);
            Debug.Log("LookDir from PlayerIndex " + playerIndex + ": " + i.LookDir);
        }
    }
}
