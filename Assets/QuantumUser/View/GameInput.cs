using Photon.Deterministic;
using Unity.VisualScripting;
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

        public bool IsPad;
        public int PadIndex;
        public InputActionAsset ActionAsset;
        public InputActions Inputs;
        public bool DebugVectorsEnabled = false;

        private InputActionMap actionMap;

        void OnEnable()
        {
            if (!IsPad)
                return;
            
            var activeController = Gamepad.all[PadIndex];
            actionMap = ActionAsset.actionMaps[0].Clone();
            actionMap.ApplyBindingOverridesOnMatchingControls(activeController);
        }

        public Quantum.Input GetInputs(Quantum.Input i, bool isMouse)
        {
            i.IsMouseInput = isMouse;
            
            i.MoveDir = actionMap.FindAction(Inputs.Movement.name).ReadValue<Vector2>().ToFPVector2();
            i.LookDir = actionMap.FindAction(Inputs.Look.name).ReadValue<Vector2>().ToFPVector2();
            
            if (DebugVectorsEnabled)
                LogVectors(i);

            i.Attack = actionMap.FindAction(Inputs.Attack.name).IsPressed();
            i.Block = actionMap.FindAction(Inputs.Block.name).IsPressed();
            i.Turn = actionMap.FindAction(Inputs.Turn.name).IsPressed();

            return i;
        }
        
        private void LogVectors(Quantum.Input i)
        {
            Debug.Log("MoveDir: " + i.MoveDir);
            Debug.Log("LookDir: " + i.LookDir);
        }
    }
}