using System;

using BrandonUtils.Standalone.Exceptions;

using Code.Runtime.Bathymetry;

using UnityEngine;

namespace Code.Runtime {
    public class GameManager : MonoBehaviour {
        public static GameManager Single;
        public        bool        isGameOver = false;

        //region Player Positioning
        public Transform                          SpawnPoint;
        public GameObject                         PlayerPrefab;
        public Transform                          PlayerInstance { get; private set; }
        public float                              StartHeight  = 10;
        public Lazy<BenthicProfile.SurveyResults> PlayerSurvey = NewSurveyPlan();
        //endregion

        public  int   lvl;
        public  float _lvlUpConditionCheckTime     = 0;
        private float _lvlUpConditionCheckInterval = 5f;

        // A lazy reference to the Coaster instance - an experiment with a different pattern than a simple Singleton
        public readonly Lazy<Coaster> LazyCoaster = new Lazy<Coaster>(() => Single.transform.parent.GetComponentInChildren<Coaster>());

        private void Awake() {
            Single = this;
            LazyCoaster.Value.PlantFakeTreesInEveryZone();
        }

        private static Lazy<BenthicProfile.SurveyResults> NewSurveyPlan() {
            return new Lazy<BenthicProfile.SurveyResults>(() => Single.LazyCoaster.Value.Survey(Single.PlayerInstance));
        }

        private void LateUpdate() {
            PlayerSurvey = NewSurveyPlan();
        }

        // Start is called before the first frame update
        void Start() {
            // subscribe to the event manager
            SubscribeToEvents();
            StartGame();
        }

        private void SubscribeToEvents() {
            EventManager.Single.ONTriggerFirstCatch     += DecideGameOverWin;
            EventManager.Single.ONTriggerCollisionShark += DecideGameOverFail;
            EventManager.Single.ONTriggerLevelUp        += LevelUp;
        }

        protected void OnDestroy() {
            // unsubscribe from the event manager
            EventManager.Single.ONTriggerFirstCatch     -= DecideGameOverWin;
            EventManager.Single.ONTriggerCollisionShark -= DecideGameOverFail;
            EventManager.Single.ONTriggerLevelUp        -= LevelUp;
        }

        // Update is called once per frame
        void Update() {
            if (Time.time > _lvlUpConditionCheckTime) {
                if (HookBehaviour.Single.checkLevelUpCondition()) {
                    EventManager.Single.TriggerLevelUp(lvl + 1);
                }

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

        private void LevelUp(int newLvl) {
            lvl = newLvl < 0 ? lvl + 1 : newLvl;
        }

        public void StartGame() {
            SpawnPlayer();
            _lvlUpConditionCheckTime = Time.time + _lvlUpConditionCheckInterval;
        }

        public void SpawnPlayer() {
            if (PlayerInstance) {
                throw new BrandonException("Can't spawn a player because there already is one!");
            }

            PlayerInstance = Instantiate(PlayerPrefab, SpawnPoint.position + (Vector3.up * StartHeight), PlayerPrefab.transform.rotation).transform;
        }
    }
}