using System.Collections.Generic;

using UnityEngine;

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

        private float _depth => Mathf.Abs(transform.localPosition.y - _depthInitial);
        private float _pressure;
        private float _depthInitial;

        private       float _velocityPull = 0;
        private       float _velocityPush = 0;
        private const float PullModifier  = 0.001f;
        private const float PushModifier  = 0.001f;
        private const float MaxDepth      = 1000f;

        private Quaternion _rotationInitial;
        public  float      StabilizerSmoothness;

        public List<Catchables> myCatches;
        private bool caughtFish = false;

        public Vector2 _movementAxesRawInput => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        public float MaxLateralSpeed;
        public float LateralAccelerationFactor;

        public Vector2 LateralVelocity {
            get {
                var vel = MyRigidbody.velocity;
                return new Vector2(vel.x, vel.z);
            }
            set => MyRigidbody.velocity = new Vector3(value.x, MyRigidbody.velocity.y, value.y);
        }

        public float SinkVelocity {
            get => MyRigidbody.velocity.y;
            set {
                Vector3 velocity;
                MyRigidbody.velocity = new Vector3((velocity = MyRigidbody.velocity).x, value, velocity.z);
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
            EventManager.Single.ONTriggerCollisionShark     += getEaten;
        }

        private void OnDestroy() {
            // cancel all substrictions to the event manager
            EventManager.Single.ONTriggerCollisionFish      -= CollisionFish;
            EventManager.Single.ONTriggerCollisionDebris    -= CollisionDebris;
            EventManager.Single.ONTriggerCollisionCatchable -= CollisionCatchable;
            EventManager.Single.ONTriggerCollisionShark     -= getEaten;
        }

        // Update is called once per frame
        private void Update() {
            // falling down based on pressure and gravity
            _velocityPull = WaterManager.Single.computePull(_depth, MaxDepth);

            // movement in total (additive as an approximation)
            SinkVelocity = (PullModifier * _velocityPull) + (PushModifier * _velocityPush);
            var targetVelocity = _movementAxesRawInput.normalized * MaxLateralSpeed;
            LateralVelocity = Vector2.Lerp(LateralVelocity, targetVelocity, Time.deltaTime * LateralAccelerationFactor);
            
            // update data in the UI
            UIManager.Single.provideData("depth", (int) (_depth / 0.546807f));
        }

        private void OnTriggerEnter(Collider other) {
            test = 1;
            if (other.GetComponent<Catchables>() != null) {
                test = 2;
                EventManager.Single.TriggerCollisionCatchable(other.GetComponent<Catchables>());
            }
        }

        private void CollisionFish(FishBehaviour fish) {
            if (!caughtFish) EventManager.Single.TriggerFirstCatch();
            // fish-specific collision stuff
        }

        private void CollisionDebris(DebrisBehaviour debris) {
            // debris-specific collision stuff
        }

        private void CollisionCatchable(Catchables newCatch) {
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

        public void getEaten() {
            gameObject.SetActive(false);
        }

        public bool checkLevelUpCondition()
        {
            return 2* Mathf.Pow(3, GameManager.Single.lvl) *_depth > (Mathf.Pow(3, GameManager.Single.lvl) + 1) * MaxDepth;
        }

    }
}
