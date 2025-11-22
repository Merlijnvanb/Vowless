using UnityEngine;
using Photon.Deterministic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

namespace Quantum
{
    public class QuantumInput : MonoBehaviour
    {
        private GameObject p1;
        private GameObject p2;

        private bool p1Joined;
        private bool p2Joined;

        public void OnJoin(PlayerInput playerInput)
        {
            Debug.Log(playerInput.name);
            playerInput.gameObject.transform.parent = transform;

            if (!p1Joined)
            {
                p1 = playerInput.gameObject;
                p1Joined = true;
                
                var gameInput = p1.GetComponent<GameInput>();
                gameInput.PlayerIndex = 0;
                return;
            }

            if (!p2Joined)
            {
                p2 = playerInput.gameObject;
                p2Joined = true;
                
                var gameInput = p2.GetComponent<GameInput>();
                gameInput.PlayerIndex = 1;
                return;
            }
        }
    }
}
