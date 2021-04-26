using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Runtime {
    /// <summary>
    /// <b>NOTE:</b> There is a <a href="https://forum.unity.com/threads/control-start-index-out-of-range-on-composite-error-on-diagonal-inputs.1004230/">known bug</a>
    /// that causes a "Control index out of range on composite" error whenever you press two keys at once.
    ///
    /// Holy fuck how did they not catch that.
    /// </summary>
    public class HookBehaviour : MonoBehaviour {
        public        float         test = 0f;
        public static HookBehaviour Single;
        public        Rigidbody     MyRigidbody;
        public        Rigidbody     MouthCollector;

        private float _depth;
        private float _pressure;
        private float _depthInitial;

        private       Vector3 _velocityPull  = new Vector3(0, 0, 0);
        private       Vector3 _velocityPush  = new Vector3(0, 0, 0);
        private       Vector3 _directionPush = new Vector3(0, 0, 0);
        private const float   PullModifier   = 0.001f;
        private const float   PushModifier   = 0.001f;
        private const float   MaxDepth       = 1000f;

        private Quaternion _rotationInitial;
        public  float      StabilizerSmoothness;

        public List<Catchables> myCatches;

        public Vector2 _movementAxesRawInput;

        public float MaxLateralSpeed;
        public float LateralAccelerationFactor;

        public Vector2 LateralVelocity {
            get {
                var vel = MyRigidbody.velocity;
                return new Vector2(vel.x, vel.z);
            }
            set => MyRigidbody.velocity = new Vector3(value.x, MyRigidbody.velocity.y, value.y);
        }

        public float VerticalVelocity {
            get => MyRigidbody.velocity.y;
            set {
                Vector3 velocity;
                velocity = new Vector3((velocity = MyRigidbody.velocity).x, value, velocity.z);
            }
        }

        private void Awake() {
            Single = this;
        }

        // Start is called before the first frame update
        private void Start() {
            var transform1 = transform;
            _depthInitial    = transform1.localPosition.y;
            _rotationInitial = transform1.rotation;

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

        /// <summary>
        /// Called by the Player Action Component whenever the designated input occurs
        /// </summary>
        /// <param name="context">Black Magic. Contains info regarding inputs and axes</param>
        [UsedImplicitly]
        public void OnMovement(InputAction.CallbackContext context) {
            setRawInput(context.ReadValue<Vector2>());
            Debug.LogWarning($"MOVED: {_movementAxesRawInput}");
        }

        public void setRawInput(Vector2 vector2) {
            if (vector2 != Vector2.zero) {
                _movementAxesRawInput = vector2;
            }
        }

        public Vector3 CalculateLateralAcceleration() {
            Debug.Log($"Calculating lateral acceleration based on movement axes: {_movementAxesRawInput}");
            var targetVelocity = _movementAxesRawInput.normalized * MaxLateralSpeed;
            return Vector2.Lerp(LateralVelocity, targetVelocity, Time.deltaTime * LateralAccelerationFactor);
        }


        // Update is called once per frame
        void Update() {
            // falling down based on pressure and gravity
            _depth          = Mathf.Abs(transform.localPosition.y - _depthInitial);
            _velocityPull.y = WaterManager.Single.computePull(_depth, MaxDepth);

            // movement in total (additive as an approximation)
            var moveVector = (PullModifier * _velocityPull + PushModifier * _velocityPush);
            moveVector           += CalculateLateralAcceleration();
            MyRigidbody.velocity =  moveVector;
        }

        private void OnTriggerEnter(Collider other) {
            test = 1;
            if (other.GetComponent<Catchables>() != null) {
                test = 2;
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
            test = 3;
            // general reaction to having caught a catchable: Yay!
            if (myCatches.Count == 0) {
                EventManager.Single.TriggerFirstCatch();
            }

            myCatches.Add(newCatch);
        }

        public Rigidbody FindHook() {
            return MouthCollector;
        }

        private void ApplyStabilization() {
            // Calculate the desired rotation, which will incorporate a bit of wobble
            var targetRotation = new Vector3(
                0,
                90,
                0
            );

            transform.rotation = Quaternion.Slerp(transform.rotation, _rotationInitial, Time.deltaTime * StabilizerSmoothness);
        }
    }
}
