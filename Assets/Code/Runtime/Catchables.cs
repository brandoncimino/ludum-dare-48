using JetBrains.Annotations;

using UnityEngine;

namespace Code.Runtime {
    //[RequireComponent(typeof(Rigidbody))]
    public class Catchables : MonoBehaviour
    {

        protected bool _gotCaught = false;
        public bool isSubmarine = false;
        
        [CanBeNull]
        protected CharacterJoint myMouth = null;
        public Vector3   MouthPosition = new Vector3(0, 0, 0.5f);
        public Rigidbody catchableRigidbody;

        protected void Start() {
            
            // find your Rigidbody
            if (catchableRigidbody == null) {
                catchableRigidbody = GetComponent<Rigidbody>();
            }

            // subscribe to the event manager
            EventManager.Single.ONTriggerCollisionCatchable += GotCaught;

            // individual behaviour depending on subclass
            myStartBehaviour();
        }

        protected void OnDestroy() {
            // unsubscribe to the event manager
            EventManager.Single.ONTriggerCollisionCatchable -= GotCaught;
        }

        protected virtual void GotCaught(Catchables newCatch) {
            if (newCatch == this)
            {
                _gotCaught = true;
                FindMyMouth();
                myMouth.connectedBody = isSubmarine? HookBehaviourSubmarine.Single.FindHook() : HookBehaviour.Single.FindHook();
                myPersonalTrigger();
            }
        }

        protected virtual void myStartBehaviour() {
            // individual Start routines of the subclasses
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

        protected void HaveIBeenEaten(Vector3 sharkPosition)
        {
            var distance = sharkPosition - transform.position;
            if (distance.magnitude < 1e-2)
            {
                transform.position -= 1e3f * transform.up;
                gameObject.SetActive(false);
            }
        }
        
        public void getEaten()
        {
            gameObject.SetActive(false);
        }
    }
}
