using JetBrains.Annotations;

using UnityEngine;

namespace Code.Runtime {
    [RequireComponent(typeof(Rigidbody))]
    public class Catchables : MonoBehaviour {
        [CanBeNull]
        protected CharacterJoint myMouth = null;
        public Vector3   MouthPosition = new Vector3(0, 0, 0.5f);
        public Rigidbody catchableRigidbody;

        protected void Start() {
            if (catchableRigidbody == null) {
                catchableRigidbody = GetComponent<Rigidbody>();
            }

            // subscribe to the event manager

            EventManager.Single.ONTriggerCollisionCatchable += GotCaught;

            myStartBehaviour();
        }

        protected void OnDestroy() {
            // unsubscribe to the event manager
            EventManager.Single.ONTriggerCollisionCatchable -= GotCaught;
        }

        protected void GotCaught(Catchables newCatch) {
            if (newCatch == this) {
                FindMyMouth();
                myMouth.connectedBody = HookBehaviour.Single.FindHook();
                myPersonalTrigger();

                // just to show that something is happening
                //transform.eulerAngles = 90f * Vector3.left;
            }
        }

        protected virtual void myStartBehaviour() {
            // nothing has to happen
        }

        protected virtual void myPersonalTrigger() {
            // subclass dependent trigger
        }

        protected virtual void FindMyMouth() {
            if (myMouth != null) return;

            // create joint
            gameObject.AddComponent<CharacterJoint>();
            myMouth = gameObject.GetComponent<CharacterJoint>();

            // configure the settings of the joint
            myMouth.autoConfigureConnectedAnchor = false;
            myMouth.anchor                       = MouthPosition;
            myMouth.massScale                    = 1e5f;
            myMouth.connectedAnchor              = Vector3.zero;

            // set angular swing limits
            var angleLimits = new SoftJointLimit();
            angleLimits.limit      = 45f;
            myMouth.highTwistLimit = angleLimits;
            myMouth.lowTwistLimit  = angleLimits;

            // let it dangle down using gravity!
            catchableRigidbody.useGravity  = true;
            catchableRigidbody.isKinematic = false;
        }
    }
}
