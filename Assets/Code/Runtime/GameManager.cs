using UnityEngine;

namespace Code.Runtime {
    public class GameManager : MonoBehaviour {
        public static GameManager Single;
        public        bool        isGameOver = false;

        private void Awake() {
            Single = this;
        }

        // Start is called before the first frame update
        void Start() {
            // subscribe to the event manager
            EventManager.Single.ONTriggerFirstCatch += DecideGameOver;
            EventManager.Single.ONTriggerCollisionShark += DecideGameOver;
            // in the min. version the game is over on the first catch
        }

        protected void OnDestroy() {
            // unsubscribe from the event manager
            EventManager.Single.ONTriggerFirstCatch -= DecideGameOver;
            EventManager.Single.ONTriggerCollisionShark += DecideGameOver;
        }

        // Update is called once per frame
        void Update() { }

        private void DecideGameOver() {
            isGameOver = true;
            EventManager.Single.TriggerGameOver();
        }
    }
}
