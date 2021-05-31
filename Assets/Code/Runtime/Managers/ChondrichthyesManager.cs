using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;

using Code.Runtime.Bathymetry;
using Code.Runtime.Bathymetry.Measurements;

using UnityEngine;
using UnityEngine.Serialization;

using Random = UnityEngine.Random;

namespace Code.Runtime.Managers {
    public class ChondrichthyesManager : MonoBehaviour {
        [Range(0.1f, 1f)]
        public float minTimeTilNextSpawn;
        [Range(0.1f, 1f)]
        public float maxTimeTilNextSpawn;
        public Vector2 RangeTimeTilNextSpawn => new Vector2(minTimeTilNextSpawn, maxTimeTilNextSpawn);
        public float   DeactivateFishBehindBy = 50;
        public float   ActivateFishAheadBy    = 100;

        #region Fishtrobution

        [Header("Fishtrobution")]
        [Tooltip("The weighted distribution used to select which FISH (not shark) will be spawned.")]
        public List<Pair<Catchables, int>> Fishtrobution;
        public List<Pair<Catchables, int>> Sharktrobution;

        [Header("Below = " + nameof(Fishtrobution) + ", above = " + nameof(Sharktrobution))]
        public AnimationCurve DangerCurve;

        [FormerlySerializedAs("FishtrobutionCurve")]
        [Tooltip("Controls how densely fish are distributed away the center of the terrain.")]
        public AnimationCurve FishtrobutionBreadthCurve;
        [Tooltip("Controls how densely fish are distributed away from the starting line.")]
        public AnimationCurve FishtrobutionDistanceCurve;
        public float   FishtrobutionRadius             = 0.1f;
        public int     InitialFishtrobution            = 700;
        public Vector2 FishtrobutionDistanceFromGround = new Vector2(5, 15);

        #endregion

        /// <summary>
        /// The arc that fish will spawn with, relative to the ground.
        /// </summary>
        public Vector2 FishSpawnPitchRange = new Vector2(-20, 45);

        public Vector2 DistanceFromCameraRange;


        public int gameLevel => GameManager.Single.lvl;

        /// <summary>
        /// The <see cref="Time.time"/> that <see cref="ForeseeFish"/> was last called
        /// </summary>
        private float lastSpawnTimeStamp;
        private float timeTilNextSpawn;

        public int FishNearPlayer;

        // Start is called before the first frame update
        private void Start() {
            lastSpawnTimeStamp = Time.time;
            GenerateFishTicket();
            Fishtrobute(InitialFishtrobution);
        }

        private List<FutureFish> Fishtrobute(int clutchSize) {
            return ForeseeFish(FishtrobutionPoint, clutchSize);
        }

        private BenthicPoint FishtrobutionPoint() {
            var distance = FishtrobutionDistanceCurve.Evaluate(Random.value);
            var breadth  = RandomNear(0.5f, FishtrobutionRadius, FishtrobutionBreadthCurve);
            return GameManager.Single.LazyCoaster.Value.BenthicPointOf(distance, breadth);
        }

        /// <summary>
        /// Determine when the next fish will be spawned in
        /// </summary>
        private void GenerateFishTicket() {
            lastSpawnTimeStamp = Time.time;
            timeTilNextSpawn   = Random.Range(minTimeTilNextSpawn, maxTimeTilNextSpawn);
        }

        // Update is called once per frame
        private void Update() {
            CheckFishActivation();
            CheckFishDeactivation();

            FishNearPlayer = FutureFish.Portents.Count(it => ShouldActivateFish(it));

            //FishNearPlayer = FutureFish.Portents.Count(it => Mathf.Abs(it.ToWorldly().z - GameManager.Single.PlayerInstance.position.z) < 20);

            // LogUtils.Log(Census());
            // if (GameManager.Single.PlayerInstance) {
            //     if (ShouldSpawn()) {
            //         // SpawnFish(GetSpawnPosition_NearPlayer(DistanceFromCameraRange));
            //     }
            // }
            // else {
            //     Debug.Log("NO PLAYER");
            // }
        }

        /// <returns>True if it's time to spawn a new <see cref="Catchables"/></returns>
        private bool ShouldSpawn() {
            return Time.time >= lastSpawnTimeStamp + timeTilNextSpawn;
        }

        /// <summary>
        /// Portends a <see cref="FutureFish"/>.
        /// </summary>
        /// <remarks>
        /// TODO: Should this be named "Fishsee"? Or is that too vague?
        /// </remarks>
        /// <param name="spawnBenthicPosition"></param>
        /// <returns></returns>
        private FutureFish ForeseeFish(Spacey.IBenthic spawnBenthicPosition) {
            var fishToSpawn = PickPossiblyDangerousFish(spawnBenthicPosition);

            // fish have a different size as a motivation to go deep

            var newFish = new FutureFish(
                fishToSpawn,
                spawnBenthicPosition,
                Random.Range(FishtrobutionDistanceFromGround.x, FishtrobutionDistanceFromGround.y),
                GetRandomFishSpawnRotation(),
                fishToSpawn.SetScaleByDepth(spawnBenthicPosition)
            );

            GenerateFishTicket();

            return newFish;
        }

        private void CheckFishActivation() {
            FutureFish.Portents.Where(ff => !ff.Activated)
                      .TakeWhile(IsCloseAhead)
                      .SkipWhile(IsFarBehind)
                      .ToList()
                      .ForEach(ff => ff.Activate());
        }

        private void CheckFishDeactivation() {
            FutureFish.Portents.Where(ff => ff.Activated)
                      .TakeWhile(IsFarBehind)
                      .ToList()
                      .ForEach(ff => ff.Deactivate());
        }

        private bool IsFarBehind(Spacey.IWorldly position) {
            return GameManager.Single.PlayerInstance.transform.position.z - position.Worldly.z > Mathf.Abs(DeactivateFishBehindBy);
        }

        private bool IsCloseAhead(Spacey.IWorldly position) {
            var player = GameManager.Single.PlayerInstance.transform.position;

            return position.Worldly.z - player.z < ActivateFishAheadBy;
        }

        private bool ShouldActivateFish(FutureFish fish) {
            return !fish.Activated && IsCloseAhead(fish) && !IsFarBehind(fish);
        }

        private bool ShouldDeactivateFish(Spacey.IWorldly fishPosition) {
            return IsFarBehind(fishPosition);
        }

        private List<FutureFish> ForeseeFish(Func<Spacey.IBenthic> spawnPositionSupplier, int clutchSize) {
            return Enumerable.Range(0, clutchSize).Select(i => ForeseeFish(spawnPositionSupplier.Invoke())).ToList();
        }

        /// <summary>
        /// TODO: Convert to return <see cref="Spacey.IWorldly"/>
        /// </summary>
        /// <param name="distanceFromCameraRange"></param>
        /// <returns></returns>
        private Vector3 GetSpawnPosition_NearPlayer(Vector2 distanceFromCameraRange) {
            var yawFromCamera       = Random.Range(-90f,                      90f);
            var distanceFromCamera  = Random.Range(distanceFromCameraRange.x, distanceFromCameraRange.y);
            var camTf               = GameManager.Single.PlayerInstance.transform;
            var directionFromCamera = Quaternion.AngleAxis(yawFromCamera, Vector3.up) * camTf.forward;
            var positionFromCamera  = directionFromCamera * distanceFromCamera;
            var worldPosition       = camTf.transform.position + positionFromCamera;
            worldPosition.y = GameManager.Single.LazyCoaster.Value.CoastlineTerrain.SampleHeight(worldPosition) + Random.Range(5, 15);
            return worldPosition;
        }

        private static float RandomNear(float center, float radius, AnimationCurve curve) {
            var direction = Random.value > 0.5 ? 1 : -1;
            var magnitude = curve.Evaluate(Random.value);
            return center + (magnitude * radius * direction);
        }

        /// <summary>
        /// Decides in what direction fish will spawn facing.
        ///
        /// The fish will spawn with a yaw between 0-360, and a pitch within <see cref="FishSpawnPitchRange"/>.
        ///
        /// </summary>
        /// <remarks>
        /// Note that due to <see cref="FishBehaviour.changeDirectionToHorizonntal">enthalpy</see>, fish will eventually all be swimming level.
        ///
        /// Note 2: I checked, and see that <a href="https://en.wikipedia.org/wiki/Enthalpy">enthalpy</a> is <b><i>not</i></b> the opposite of <a href="https://en.wikipedia.org/wiki/Entropy">entropy</a>.
        /// I am very sad.</remarks>
        /// <returns></returns>
        private Quaternion GetRandomFishSpawnRotation() {
            var yaw   = Random.Range(0,                     360);
            var pitch = Random.Range(FishSpawnPitchRange.x, FishSpawnPitchRange.y);
            return Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, Vector3.right);
        }

        private Catchables PickPossiblyDangerousFish(Spacey.IBenthic benthicPoint) {
            var dangerChance      = DangerCurve.Evaluate(benthicPoint.Distance);
            var shouldBeDangerous = Random.value < dangerChance;
            var spawnList         = shouldBeDangerous ? Sharktrobution : Fishtrobution;
            return PickRandomCatchable(spawnList);
        }

        private Catchables PickRandomCatchable(List<Pair<Catchables, int>> distributionList) {
            if (distributionList == null || distributionList.Count == 0) {
                throw new BrandonException($"The {nameof(distributionList)} list is empty!");
            }

            var totalWeight       = distributionList.Sum(it => it.Y);
            var weightedSelection = Random.Range(0, totalWeight);

            // Some people talk about fancier, smarter ways to do this here https://stackoverflow.com/questions/7366838/select-a-random-item-from-a-weighted-list
            var cumulative = 0;
            foreach (var fish in distributionList) {
                cumulative += fish.Y;
                if (cumulative > weightedSelection) {
                    return fish.X;
                }
            }

            throw new BrandonException(
                $"Unable to {nameof(PickRandomCatchable)}!\n" +
                $"{nameof(totalWeight)}:       {totalWeight}\n" +
                $"{nameof(weightedSelection)}: {weightedSelection}\n"
            );
        }

        private Dictionary<object, object> Census() {
            return new Dictionary<object, object>() {
                {$"{nameof(FutureFish.Portents)}", FutureFish.Portents.Count},
                {"Instantiated", FutureFish.Portents.Count(it => it.Instantiated)},
                {"Activated", FutureFish.Portents.Count(it => it.Activated)}
            };
        }
    }
}
