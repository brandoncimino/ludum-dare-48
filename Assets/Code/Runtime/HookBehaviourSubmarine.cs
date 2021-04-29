using System.Collections.Generic;

using UnityEngine;

namespace Code.Runtime {
    public class HookBehaviourSubmarine : HookBehaviour {
        public float test = 0;

        

        #region push-related variables

        private float _pushAngleMax   = 45f;
        private float _pushAngleSpeed = 30f;

        private float   _pushSpeed         = 10f;
        public  Vector3 _directionPush     = new Vector3(0, 0,  0);
        public  Vector3 _directionOriginal = new Vector3(0, -1, 0);
        private float   PushModifier       = 1f;

        #endregion

        #region lateral movement variables

        private float _lateralAngleMax   = 115f;
        private float _lateralAngleSpeed = 90f;

        #endregion

        #region stabilization related variables

        private float _stabilizationSpeed = 10f;

        #endregion
        


        // Update is called once per frame
        protected override void Update() {
            var horizontalRotation = new Quaternion();

            if (RawMovement.magnitude < 0.5f) {
                // stabilization
                horizontalRotation.eulerAngles = new Vector3(-60, transform.rotation.eulerAngles.y, 0);
                transform.rotation             = Quaternion.RotateTowards(transform.rotation, horizontalRotation, _stabilizationSpeed * Time.deltaTime);
            }
            else {
                // combined movement
                horizontalRotation.eulerAngles = new Vector3(-60 + RawMovement.z * _pushAngleMax, transform.rotation.eulerAngles.y, 0 + RawMovement.x * _lateralAngleMax);
                transform.rotation             = Quaternion.RotateTowards(transform.rotation, horizontalRotation, _pushAngleSpeed * Time.deltaTime);

            }

            // user-induced movement
            _directionPush     =  (transform.rotation * _directionOriginal);
            PushModifier       =  1f - 0.3f * Mathf.Abs(RawMovement.x) + RawMovement.z;
            // transform.position += (PushModifier * _pushSpeed * Time.deltaTime) * _directionPush;

            // pull: falling downward based on pressure and gravity
            _velocityPull      = WaterManager.Single.computePull(_depth, MaxDepth);
            MyRigidbody.velocity = (PullModifier * _velocityPull) * Vector3.up + (PushModifier * _pushSpeed) * _directionPush;
            
            // update data in the UI
            UIManager.Single.provideData("depth", (int) (_depth / 0.546807f));
            
        }

    }
}
