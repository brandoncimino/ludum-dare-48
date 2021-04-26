using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Runtime {
    public class HookControl : MonoBehaviour {
        [Header("Input Settings")]
        public PlayerInput playerInput;

        public  float   movementSmoothing;
        public Vector3 _rawInputMovement;
        private Vector3 _smoothInputMovement;

        private Rigidbody _myRigidbody;

        public bool isSubmarine = false;
        [CanBeNull] public HookBehaviourSubmarine Submarine;

        private void Start() {
            _myRigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Called by the Player Action Component whenever the designated input occurs
        /// </summary>
        /// <param name="context">Black Magic. Contains info regarding inputs and axes</param>
        public void OnMovement(InputAction.CallbackContext context) {
            Vector2 inputMovement = context.ReadValue<Vector2>();
            Debug.Log(inputMovement);
            _rawInputMovement = new Vector3(inputMovement.x, 0, inputMovement.y);
        }

        private void FixedUpdate()
        {
            if (isSubmarine) return;
            
            CalculateMovementInputSmoothing();
            UpdatePlayerMovement();
        }

        /// <summary>
        /// Smooth out the raw movement input information for cleaner movement
        /// </summary>
        private void CalculateMovementInputSmoothing() {
            //Unimplemented via passthrough. No actual changes are made to the raw input.
            _smoothInputMovement = _rawInputMovement;
        }

        /// <summary>
        /// Moves the Hook based on the smoothed input information
        /// </summary>
        private void UpdatePlayerMovement() {
            _myRigidbody.velocity += _smoothInputMovement * Time.deltaTime;
        }

        /// <summary>
        /// Given the current buttons pushed within WASD, return a Vector2 where A&D control left and right, and W&S control up and down respectively.
        /// </summary>
        /// <param name="keyboard">Input Manager's keyboard. This is the current keyboard</param>
        /// <returns>A Normalized Vector2 based on WASD input</returns>
        [Obsolete("This uses the Direct Input methodology. Please use the Player Input Component instead")]
        private Vector3 DirectInputToVector3(Keyboard keyboard) {
            return new Vector2(
                keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue(),
                keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue()
            );
        }
    }
}
