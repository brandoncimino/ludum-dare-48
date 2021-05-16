using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Vectors;

using UnityEngine;

namespace Code.Runtime {
    public class HookBehaviourSubmarine : HookBehaviour {
        #region push-related variables

        private float _pitchMax   = 45f;
        private float _pitchSpeed = 10f;

        private float   _pushSpeed         = 10f;
        public  Vector3 _directionPush     = new Vector3(0, 0,  0);
        public  Vector3 _directionOriginal = new Vector3(0, -1, 0);
        private float   PushModifier       = 1f;

        #endregion

        #region lateral movement variables

        private float _yawMax = 115f;

        /**
         * In degrees/second
         */
        private float _yawSpeed = 20f;

        #endregion

        #region stabilization related variables

        private float _stabilizationSpeed = 1f;

        #endregion

        // Update is called once per frame
        protected override void Update() {
            var horizontalRotation = new Quaternion();

            // if (RawMovement.magnitude < 0.5f) {
            // stabilization
            // horizontalRotation.eulerAngles = new Vector3(-60, transform.rotation.eulerAngles.y, 0);
            // transform.rotation             = Quaternion.RotateTowards(transform.rotation, horizontalRotation, _stabilizationSpeed * Time.deltaTime);
            ApplyStabilization(_stabilizationSpeed);
            // }
            // else {
            // combined movement
            // horizontalRotation.eulerAngles = new Vector3(-60 + RawMovement.y * _pushAngleMax, transform.rotation.eulerAngles.y, 0 + RawMovement.x * _lateralAngleMax);
            // transform.rotation             = Quaternion.RotateTowards(transform.rotation, horizontalRotation, _pushAngleSpeed * Time.deltaTime);

            // Yaw
            // Calculate the desired yaw relative to straight-forward
            var yawTarget = RawMovement.x * _yawMax;
            // Convert that yaw to a Quaternion (which is relative to straight-forward)
            var yawTargetRotation = Quaternion.Euler(0, yawTarget, 0);

            // Calculate the yaw delta
            var yawDelta = Quaternion.RotateTowards(Quaternion.identity, yawTargetRotation, _yawSpeed * Time.deltaTime);

            // Calculate the desired pitch relative to the ground pitch
            var pitchTargetRelative = RawMovement.y * _pitchMax;
            var pitchTerrain        = GetTerrainPitch();
            var pitchTarget         = pitchTerrain + pitchTargetRelative;
            var pitchTargetRotation = Quaternion.Euler(pitchTarget, 0, 0);

            // Calculate the pitch delta
            var pitchDelta = Quaternion.RotateTowards(Quaternion.identity, pitchTargetRotation, _pitchSpeed * Time.deltaTime);

            // Apply the deltas?
            transform.rotation *= yawDelta * pitchDelta;
            // }

            // user-induced movement
            //_directionPush     =  (transform.rotation * _directionOriginal);
            // PushModifier = 1f - 0.3f * Mathf.Abs(RawMovement.x) + RawMovement.y;
            PushModifier = Mathf.Lerp(1f, 0.7f, 0.5f - RawMovement.y);
            // transform.position += (PushModifier * _pushSpeed * Time.deltaTime) * _directionPush;

            // pull: falling downward based on pressure and gravity
            _velocityPull = WaterManager.Single.computePull(_depth, MaxDepth);
            var sinkVelocity    = PullModifier * _velocityPull * Vector3.up;
            var forwardVelocity = transform.forward * (_pushSpeed * PushModifier);
            MyRigidbody.velocity = sinkVelocity + forwardVelocity;

            // update data in the UI
            UIManager.Single.provideData("depth", (int) (_depth / 0.546807f));
        }

        private void ApplyStabilization(float stabilizationFactor) {
            var hoverHit        = Hovercast();
            var terrainRotation = Quaternion.FromToRotation(Vector3.up, hoverHit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, terrainRotation, Time.deltaTime * stabilizationFactor);
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
