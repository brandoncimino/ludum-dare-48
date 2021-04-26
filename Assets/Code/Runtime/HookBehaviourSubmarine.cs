using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Runtime
{
    public class HookBehaviourSubmarine : MonoBehaviour
    {
        public float test = 0;
        
        #region public
        public static HookBehaviourSubmarine Single;
        public        Rigidbody     MyRigidbody;
        public        Rigidbody     MouthCollector;
        #endregion
        
        #region depth-related variables
        private float _depth;
        private float _depthInitial;
        private const float   MaxDepth       = 200f;
        #endregion
        
        #region pull-related variables
        private       Vector3 _velocityPull  = new Vector3(0, 0, 0);
        private const float   PullModifier   = 1e-6f;
        #endregion
        
        #region input-related variables
        public Vector3 RawMovement;
        public HookControl myControlEngine;
        #endregion
        
        #region push-related variables
        private float _pushAngleMax = 45f;
        private float _pushAngleSpeed = 30f;

        private float _pushSpeed = 10f;
        public       Vector3 _directionPush = new Vector3(0, 0, 0);
        public       Vector3 _directionOriginal = new Vector3(0, -1, 0);
        private float   PushModifier   = 1f;
        #endregion
        
        #region lateral movement variables
        private float _lateralAngleMax = 115f;
        private float _lateralAngleSpeed = 90f;
        #endregion

        #region stabilization related variables

        private float _stabilizationSpeed = 10f;
        

        #endregion
        
        #region catch-related variables
        public List<Catchables> myCatches = new List<Catchables>() { };
        #endregion
        
        private void Awake() {
            Single = this;
        }
        
        // Start is called before the first frame update
        private void Start() {
            _depthInitial    = transform.localPosition.y;

            // substrictions to the event manager
            EventManager.Single.ONTriggerCollisionFish      += CollisionFish;
            EventManager.Single.ONTriggerCollisionDebris    += CollisionDebris;
            EventManager.Single.ONTriggerCollisionCatchable += CollisionCatchable;
        }
        
        private void OnDestroy() {
            // cancel all substrictions to the event manager
            EventManager.Single.ONTriggerCollisionFish      -= CollisionFish;
            EventManager.Single.ONTriggerCollisionDebris    += CollisionDebris;
            EventManager.Single.ONTriggerCollisionCatchable -= CollisionCatchable;
        }
        
        // Update is called once per frame
        void Update()
        {

            RawMovement = myControlEngine._rawInputMovement;
            
            // stabilization
            var horizontalRotation = new Quaternion();
            horizontalRotation.eulerAngles = new Vector3(-30, transform.rotation.eulerAngles.y, 0);
            transform.rotation             = Quaternion.RotateTowards(transform.rotation, horizontalRotation, _stabilizationSpeed * Time.deltaTime);
            
            // push: acceleration and deceleration in front-back direction
            horizontalRotation.eulerAngles = new Vector3(-30 + RawMovement.z * _pushAngleMax, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.rotation             = Quaternion.RotateTowards(transform.rotation, horizontalRotation, _pushAngleSpeed * Time.deltaTime);
            
            // lateral movement
            horizontalRotation.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0 + RawMovement.x * _lateralAngleMax);
            transform.rotation             = Quaternion.RotateTowards(transform.rotation, horizontalRotation, _lateralAngleSpeed * Time.deltaTime);
            
            // user-induced movement
            _directionPush = (transform.rotation * _directionOriginal);
            PushModifier = 1 + Mathf.Abs(RawMovement.x) + RawMovement.z;
            transform.position += (PushModifier * _pushSpeed * Time.deltaTime) * _directionPush;
            
            // pull: falling downward based on pressure and gravity
            _depth          = Mathf.Abs(transform.localPosition.y - _depthInitial);
            _velocityPull.y = WaterManager.Single.computePull(_depth, MaxDepth);
            MyRigidbody.velocity = PullModifier * _velocityPull;
            
        }
        
        #region catch
        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Catchables>() != null) {
                EventManager.Single.TriggerCollisionCatchable(other.GetComponent<Catchables>());
            }
        }

        private void CollisionFish(FishBehaviour fish) {
            // fish-specific collision stuff
        }

        private void CollisionDebris(DebrisBehaviour debris) {
            // debris-specific collision stuff
        }

        private void CollisionCatchable(Catchables newCatch) {
            // general reaction to having caught a catchable: Yay!
            if (myCatches.Count == 0) {
                EventManager.Single.TriggerFirstCatch();
            }

            myCatches.Add(newCatch);
        }

        public Rigidbody FindHook() {
            return MouthCollector;
        }
        
        #endregion

        #region movement calculations
        
        #endregion
    }
}