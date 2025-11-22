using UnityEngine;
using Photon.Deterministic;

namespace Quantum
{
    public class QuantumInput : MonoBehaviour
    {
        public bool TwoPads;
        
        public GameInput KbmInput;
        public GameInput PadInput0;
        public GameInput PadInput1;
        
        public int KbmIndex = 0;
        public int Pad0Index = 1;
        public int Pad1Index = 2;
        
        void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        void Update()
        {
            if (!TwoPads && UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                var kbm = KbmIndex;
                var pad = Pad0Index;

                KbmIndex = pad;
                Pad0Index = kbm;
            }
        }

        private void PollInput(CallbackPollInput callback)
        {
            Quantum.Input i = new Quantum.Input();
            
            //Debug.Log(callback.PlayerSlot);
            if (!TwoPads)
            {
                if (callback.PlayerSlot == KbmIndex)
                    i = KbmInput.GetInputs(i, false);

                else if (callback.PlayerSlot == Pad0Index)
                    i = PadInput0.GetInputs(i, false);
            }
            else
            {
                if (callback.PlayerSlot == Pad0Index)
                    i = PadInput0.GetInputs(i, false);

                else if (callback.PlayerSlot == Pad1Index)
                    i = PadInput1.GetInputs(i, false);
            }

            callback.SetInput(i, DeterministicInputFlags.Repeatable);
        }
        
        private void DebugVectors(Quantum.Input i, int playerIndex)
        {
            Debug.Log("MoveDir from PlayerIndex " + playerIndex + ": " + i.MoveDir);
            Debug.Log("LookDir from PlayerIndex " + playerIndex + ": " + i.LookDir);
        }
    }
}
