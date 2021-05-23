using System.Linq;

using BrandonUtils.Spatial;
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
        public float _nosFactor = 10f;

        /**
         * The multiplier on <see cref="_pushSpeed"/> when the hook is pitched <b>downward</b>
         */
        public float _downwardPushModifier = 1f;
        /**
         * The multiplier on <see cref="_pushSpeed"/> when the hook is pitched <b>upward</b>
         */
        public float _upwardPushModifier = 0.7f;

        private float NosModifier  => Input.GetButton("Jump") ? _nosFactor : 1;
        private float PushModifier => Mathf.Lerp(_downwardPushModifier, _upwardPushModifier, 0.5f - RawMovement.y) + (NosModifier - 1);

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

        public float _stabilizationSpeed = 10f;

        #endregion

        // Update is called once per frame
        protected override void Update() {
            ApplyStabilization(_stabilizationSpeed);
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

        private void ApplyStabilization(float stabilizationSpeed) {
            if (RawMovement.magnitude < float.Epsilon) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, StableRotation(), Time.deltaTime * stabilizationSpeed);
            }
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
            var inputYaw        = RawMovement.x * _yawMax;
            var inputPitch      = RawMovement.y * _pitchMax;
            var terrainRotation = GetTerrainRotation();
            var pitchRotation   = Quaternion.AngleAxis(inputPitch, Vector3.right);
            var yawRotation     = Quaternion.AngleAxis(inputYaw,   Vector3.up);
            var targetRotation  = terrainRotation * yawRotation * pitchRotation;
            return targetRotation;
        }

        /**
         * <see cref="Quaternion.Slerp"/>s the hook towards the <see cref="CourseToSteer"/>
         */
        private void Steer() {
            transform.rotation = Quaternion.Slerp(transform.rotation, CourseToSteer(), _steerFactor * Time.deltaTime);
        }

        private RaycastHit Hovercast() {
            var hits = HoverHelper.ArcCast(transform, Cube.Face.Down, Cube.Face.Forward, 89, 5);
            hits.Sort((a, b) => a.distance.CompareTo(b.distance));
            return hits.First();
        }

        private Quaternion GetTerrainRotation() {
            var hoverHit = Hovercast();
            return Quaternion.FromToRotation(Vector3.up, hoverHit.normal);
        }
    }
}
