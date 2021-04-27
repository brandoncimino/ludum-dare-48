using UnityEngine;

namespace Code.Runtime {
    public class GameManager : MonoBehaviour {
        public static GameManager Single;
        public        bool        isGameOver = false;
        
        public int lvl;
        private float _lvlUpConditionCheckTime = 0;
        private float _lvlUpConditionCheckInterval = 15f;
        
        private void Awake() {
            Single = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _lvlUpConditionCheckTime = Time.time + _lvlUpConditionCheckInterval;
            
            // subscribe to the event manager
            EventManager.Single.ONTriggerFirstCatch += DecideGameOverWin;
            EventManager.Single.ONTriggerCollisionShark += DecideGameOverFail;
            EventManager.Single.ONTriggerLevelUp += LevelUp;
        }

        protected void OnDestroy() {
            // unsubscribe from the event manager
            EventManager.Single.ONTriggerFirstCatch -= DecideGameOverWin;
            EventManager.Single.ONTriggerCollisionShark += DecideGameOverFail;
            EventManager.Single.ONTriggerLevelUp -= LevelUp;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time > _lvlUpConditionCheckTime)
            {
                if (HookBehaviour.Single.checkLevelUpCondition()) EventManager.Single.TriggerLevelUp(lvl + 1);
                _lvlUpConditionCheckTime = Time.time + _lvlUpConditionCheckInterval;
            }
        }

        private void DecideGameOverWin() {
            isGameOver = true;
            EventManager.Single.TriggerGameOverWin();
        }
        
        private void DecideGameOverFail() {
            isGameOver = true;
            EventManager.Single.TriggerGameOverFail();
        }

        private void LevelUp(int newLvl)
        {
            lvl = newLvl < 0 ? lvl + 1 : newLvl;
        }
    }
}
