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

        public Quantum.Input GetInputs(Quantum.Input i)
        {
            i.MoveDir = Inputs.Movement.action.ReadValue<Vector2>().ToFPVector2();
            i.LookDir = Inputs.Look.action.ReadValue<Vector2>().ToFPVector2();

            i.Attack = Inputs.Attack.action.IsPressed();
            i.Block = Inputs.Block.action.IsPressed();
            i.Turn = Inputs.Turn.action.IsPressed();

            return i;
        }
    }
}