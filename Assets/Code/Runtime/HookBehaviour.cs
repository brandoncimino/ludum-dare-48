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
        #region general public stuff

        public static HookBehaviour Single;
        public        Rigidbody     MyRigidbody;
        protected     Quaternion    _rotationInitial;

        #endregion


        #region depth-related variables

        protected       float _depth => Mathf.Abs(transform.localPosition.y - _depthInitial);
        protected       float _depthInitial;
        protected const float MaxDepth = 1000f;

        #endregion

        #region pull-related variables

        protected float _velocityPull = 0;
        public    float PullModifier  = 1e-3f;

        #endregion


        #region catch-related variables

        public    List<Rigidbody>  MouthCollection;
        public    Rigidbody        MouthCollector;
        public    List<Catchables> myCatches  = new List<Catchables>() { };
        protected bool             caughtFish = false;

        #endregion


        #region input-related variables

        public Vector2 RawMovement => new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        public Vector2 _movementAxesRawInput => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        #endregion

        protected int TerrainMask { get; private set; }

        private void Awake() {
            Single = this;
        }

        // Start is called before the first frame update
        private void Start() {
            TerrainMask = LayerMask.NameToLayer("Terrain");

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


        protected virtual void Update() {
            // depends on the subclass
        }

        public bool checkLevelUpCondition() {
            return Mathf.Pow(3, GameManager.Single.lvl) * _depth > (Mathf.Pow(3, GameManager.Single.lvl) - Mathf.Pow(2, GameManager.Single.lvl)) * MaxDepth;
        }

        #region catch-related stuff

        protected void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Catchables>() != null) {
                EventManager.Single.TriggerCollisionCatchable(other.GetComponent<Catchables>());
            }
        }

        protected void CollisionFish(FishBehaviour fish) {
            if (!caughtFish) EventManager.Single.TriggerFirstCatch();
            // fish-specific collision stuff
        }

        protected void CollisionDebris(DebrisBehaviour debris) {
            // debris-specific collision stuff
        }

        protected void CollisionCatchable(Catchables newCatch) {
            myCatches.Add(newCatch);
        }

        public Rigidbody FindHook() {
            return MouthCollector;
        }

        public void getEaten() {
            gameObject.SetActive(false);
        }

        #endregion
    }
}
