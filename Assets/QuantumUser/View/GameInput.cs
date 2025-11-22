using Photon.Deterministic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quantum
{
    public class GameInput : MonoBehaviour
    {
        public bool IsPad;
        public int PadIndex;

        public InputActionReference Movement;
        public InputActionReference Look;
        
        public InputActionReference Attack;
        public InputActionReference Block;
        public InputActionReference Turn;
        
        public Quantum.Input GetInputs(Quantum.Input i, bool isMouse)
        {
            i.IsMouseInput = isMouse;
            
            i.MoveDir = Movement.action.ReadValue<Vector2>().ToFPVector2();
            i.LookDir = Look.action.ReadValue<Vector2>().ToFPVector2();
        
            i.Attack = Attack.action.IsPressed();
            i.Block = Block.action.IsPressed();
            i.Turn = Turn.action.IsPressed();
        
            return i;
        }
        
        private void LogVectors(Quantum.Input i)
        {
            Debug.Log("MoveDir: " + i.MoveDir);
            Debug.Log("LookDir: " + i.LookDir);
        }
    }
}