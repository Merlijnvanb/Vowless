using Photon.Deterministic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quantum
{
    public class GameInput : MonoBehaviour
    {
        [System.Serializable]
        public struct InputActions
        {
            public InputActionReference Movement;
            public InputActionReference Look;
            public InputActionReference Attack;
            public InputActionReference Block;
            public InputActionReference Turn;
        }
        
        public InputActions Inputs;
        public bool DebugVectorsEnabled = false;

        public Quantum.Input GetInputs(Quantum.Input i, bool isMouse)
        {
            i.IsMouseInput = isMouse;
            
            i.MoveDir = Inputs.Movement.action.ReadValue<Vector2>().ToFPVector2();
            i.LookDir = Inputs.Look.action.ReadValue<Vector2>().ToFPVector2();
            
            if (DebugVectorsEnabled)
                LogVectors(i);

            i.Attack = Inputs.Attack.action.IsPressed();
            i.Block = Inputs.Block.action.IsPressed();
            i.Turn = Inputs.Turn.action.IsPressed();

            return i;
        }
        
        private void LogVectors(Quantum.Input i)
        {
            Debug.Log("MoveDir: " + i.MoveDir);
            Debug.Log("LookDir: " + i.LookDir);
        }
    }
}