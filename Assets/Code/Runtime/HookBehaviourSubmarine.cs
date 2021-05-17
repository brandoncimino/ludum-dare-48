using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Vectors;

using UnityEngine;

namespace Code.Runtime {
    public class HookBehaviourSubmarine : HookBehaviour {
        #region push-related variables

        /**
         * The maximum pitch (both upward and downward)
         */
        public float _pitchMax = 45f;
        /**
         * The multiplier applied to <see cref="Time.deltaTime"/> when <see cref="Quaternion.Slerp"/>ing toward the <see cref="CourseToSteer"/>
         */
        public float _steerFactor = 2f;

        public float _pushSpeed = 10f;

        /**
         * The multiplier on <see cref="_pushSpeed"/> when the hook is pitched <b>downward</b>
         */
        public float _downwardPushModifier = 1f;
        /**
         * The multiplier on <see cref="_pushSpeed"/> when the hook is pitched <b>upward</b>
         */
        public float _upwardPushModifier = 0.7f;

        private float PushModifier => Mathf.Lerp(_downwardPushModifier, _upwardPushModifier, 0.5f - RawMovement.y);

        #endregion

        #region lateral movement variables

        /**
         * The maximum <see cref="Vector3Utils.Yaw"/> when steering (i.e. parallel to the ground)
         */
        public float _yawMax = 115f;

        #endregion

        #region stabilization related variables

        /**
         * The multiplier applied to <see cref="Time.deltaTime"/> when <see cref="Quaternion.Slerp"/>ing towards <see cref="StableRotation"/>
         */
        public float _stabilizationFactor = 1f;

        #endregion

        // Update is called once per frame
        protected override void Update() {
            ApplyStabilization(_stabilizationFactor);
            Steer();
            ApplyVelocity();

            // update data in the UI
            UIManager.Single.provideData("depth", (int) (_depth / 0.546807f));
        }

        private void ApplyVelocity() {
            // user-induced movement
            var pushModifier = PushModifier;
            // transform.position += (PushModifier * _pushSpeed * Time.deltaTime) * _directionPush;

            // pull: falling downward based on pressure and gravity
            _velocityPull = WaterManager.Single.computePull(_depth, MaxDepth);
            var sinkVelocity    = PullModifier * _velocityPull * Vector3.up;
            var forwardVelocity = transform.forward * (_pushSpeed * pushModifier);
            MyRigidbody.velocity = sinkVelocity + forwardVelocity;
        }

        private void ApplyStabilization(float stabilizationFactor) {
            transform.rotation = Quaternion.Slerp(transform.rotation, StableRotation(), Time.deltaTime * stabilizationFactor);
        }

        /**
         * The rotation that the hook returns to if there is no user input
         *
         * TODO: Find a cooler nautical term for this
         */
        private Quaternion StableRotation() {
            var hoverHit        = Hovercast();
            var terrainRotation = Quaternion.FromToRotation(Vector3.up, hoverHit.normal);
            return terrainRotation;
        }

        /**
         * Calculates the "course to steer", i.e. the desired bearing based on the player's input
         */
        private Quaternion CourseToSteer() {
            var targetYaw    = RawMovement.x * _yawMax;
            var terrainPitch = GetTerrainPitch();
            var inputPitch   = RawMovement.y * _pitchMax;
            var targetPitch  = terrainPitch + inputPitch;

            var targetRotation = Quaternion.Euler(targetPitch, targetYaw, 0);
            return targetRotation;
        }

        /**
         * <see cref="Quaternion.Slerp"/>s the hook towards the <see cref="CourseToSteer"/>
         */
        private void Steer() {
            transform.rotation = Quaternion.Slerp(transform.rotation, CourseToSteer(), _steerFactor * Time.deltaTime);
        }

        private RaycastHit Hovercast() {
            var pos      = transform.position;
            var hoverRay = new Ray(pos, Vector3.down);
            if (Physics.Raycast(hoverRay, out var hoverHit, 100, TerrainMask)) {
                return hoverHit;
            }
            else {
                throw new TransDimensionalException("The hook isn't over the terrain!");
            }
        }

        private float GetTerrainPitch() {
            var hoverHit = Hovercast();
            return Quaternion.FromToRotation(Vector3.up, hoverHit.normal).eulerAngles.Pitch();
        }
    }
}
