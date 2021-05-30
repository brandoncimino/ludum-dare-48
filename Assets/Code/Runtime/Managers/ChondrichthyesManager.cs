using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;

using Code.Runtime;
using Code.Runtime.Bathymetry.Measurements;

using UnityEngine;

using Random = UnityEngine.Random;

public class ChondrichthyesManager : MonoBehaviour {
    public Camera     cameraCrew;
    public Catchables goblinShark;
    public Catchables basicFish;

    [Range(0.1f, 1f)]
    public float minTimeTilNextSpawn;
    [Range(0.1f, 1f)]
    public float maxTimeTilNextSpawn;
    public Vector2 RangeTimeTilNextSpawn => new Vector2(minTimeTilNextSpawn, maxTimeTilNextSpawn);
    public Vector2 distanceFromCameraCrewToSpawnFish;

    [Range(0, 100)]
    public float chanceToUseGoblinShark;

    private float fishScaleMin  = 0.1f;
    private float fishScaleStep = 0.3f;

    /// <summary>
    /// The arc that fish will spawn with, relative to the ground.
    /// </summary>
    public Vector2 FishSpawnPitchRange = new Vector2(-20, 45);

    public Vector2 DistanceFromCameraRange;

    [Header("The weighted distribution used to select which fish will be spawned")]
    public List<Pair<Catchables, int>> Fishtrobution;

    /// A much simpler and probably smarter implementation of <see cref="Code.Runtime.Bathymetry.Holder"/>
    private static Lazy<Transform> FishHolder = new Lazy<Transform>(() => new GameObject(nameof(FishHolder)).transform);
    private static List<Catchables> AllFish = new List<Catchables>();

    public int gameLevel => GameManager.Single.lvl;

    /// <summary>
    /// The <see cref="Time.time"/> that <see cref="SpawnFish"/> was last called
    /// </summary>
    private float lastSpawnTimeStamp;
    private float timeTileNextSpawn;

    // Start is called before the first frame update
    private void Start() {
        lastSpawnTimeStamp = Time.time;
        GenerateFishTicket();
        SpawnStartingFish();
    }

    private void SpawnStartingFish() {
        // Enumerable.Repeat()
    }

    private Catchables SpawnFishAroundCenter() {
        return SpawnFish(
            GameManager
                .Single
                .LazyCoaster
                .Value.BenthicPointOf(
                    Random.value,
                    0.5f + Random.Range(0.1f, 0.1f)
                )
        );
    }

    /// <summary>
    /// Determine when the next fish will be spawned in
    /// </summary>
    private void GenerateFishTicket() {
        lastSpawnTimeStamp = Time.time;
        timeTileNextSpawn  = Random.Range(minTimeTilNextSpawn, maxTimeTilNextSpawn);
    }

    // Update is called once per frame
    private void Update() {
        if (GameManager.Single.PlayerInstance) {
            if (ShouldSpawn()) {
                SpawnFish(GetSpawnPosition_NearPlayer(DistanceFromCameraRange));
            }
        }
        else {
            Debug.Log("NO PLAYER");
        }
    }

    /// <returns>True if it's time to spawn a new <see cref="Catchables"/></returns>
    private bool ShouldSpawn() {
        return Time.time >= lastSpawnTimeStamp + timeTileNextSpawn;
    }

    /// <summary>
    /// Contains the "decision making", aka "game logic", around spawning a fish, such as:
    /// <ul>
    /// <li>Calling <see cref="GenerateFishTicket"/></li>
    /// <li>Deciding where to spawn the fish</li>
    /// </ul>
    ///
    /// For "engine-y" stuff, see <see cref="InstantiateFish"/>
    /// </summary>
    /// <param name="spawnWorldPosition"></param>
    /// <returns></returns>
    private Catchables SpawnFish(Vector3 spawnWorldPosition) {
        var fishToSpawn = PickRandomFish();

        // fish have a different size as a motivation to go deep
        var fishScale = fishScaleMin + fishScaleStep * (gameLevel + Random.Range(-1f, 1f));

        var newFish = InstantiateFish(fishToSpawn, spawnWorldPosition, GetRandomFishSpawnRotation(), fishScale);

        GenerateFishTicket();

        return newFish;
    }

    private Catchables SpawnFish(Spacey.IWorldly spawnPoint) {
        return SpawnFish(spawnPoint.ToWorldly());
    }

    /// <summary>
    /// Spawns a <a href="https://en.wikipedia.org/wiki/Clutch_(eggs)">clutch</a> of fish
    /// </summary>
    /// <param name="clutchSize">the size of the <a href="https://en.wikipedia.org/wiki/Clutch_(eggs)">clutch</a></param>
    /// <returns></returns>
    public List<Catchables> SpawnFish(int clutchSize) {
        return Enumerable.Repeat(SpawnFish(GetSpawnPosition_OverTerrain()), clutchSize).ToList();
    }

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

    private static float RandomNear(float center, float radius) {
        return Random.Range(center - radius, center + radius);
    }

    private Vector3 GetSpawnPosition_OverTerrain() {
        var terrain     = GameManager.Single.LazyCoaster.Value.CoastlineTerrain;
        var terrainData = terrain.terrainData;

        var distMin = GameManager.Single.PlayerInstance ? GameManager.Single.PlayerInstance.transform.position.x - 10 : terrain.GetPosition().x;

        var distRange = new Vector2(
            distMin,
            terrain.GetPosition().z + terrainData.size.z
        );

        var breadthRange = new Vector2(
            terrain.GetPosition().x,
            terrain.GetPosition().x + terrainData.size.x
        );

        var     worldDist    = Random.Range(distRange.x,    distRange.y);
        var     worldBreadth = Random.Range(breadthRange.x, breadthRange.y);
        Vector3 worldPos     = new Vector3(worldBreadth, 0, worldDist);
        worldPos.y = terrain.SampleHeight(worldPos);
        return worldPos;
    }

    /// <summary>
    /// "Instantiates" a <see cref="Catchables"/> prefab.
    ///
    /// This method should not contain any "decision making", and should only handle the "engine-y" stuff.
    /// Decision making should go in <see cref="SpawnFish"/> instead.
    /// </summary>
    /// <param name="fishToInstantiate"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="fishScale"></param>
    /// <returns></returns>
    private static Catchables InstantiateFish(Catchables fishToInstantiate, Vector3 position, Quaternion rotation, float fishScale) {
        var newFish = Instantiate(fishToInstantiate, position, rotation, FishHolder.Value);
        newFish.ScaleUp(fishScale);
        AllFish.Add(newFish);
        return newFish;
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

    private Catchables PickRandomFish() {
        if (Fishtrobution == null || Fishtrobution.Count == 0) {
            throw new BrandonException($"The {nameof(Fishtrobution)} list is empty!");
        }

        var totalWeight       = Fishtrobution.Sum(it => it.Y);
        var weightedSelection = Random.Range(0, totalWeight);

        // Some people talk about fancier, smarter ways to do this here https://stackoverflow.com/questions/7366838/select-a-random-item-from-a-weighted-list
        var cumulative = 0;
        foreach (var fish in Fishtrobution) {
            cumulative += fish.Y;
            if (cumulative > weightedSelection) {
                return fish.X;
            }
        }

        throw new BrandonException(
            $"Unable to {nameof(PickRandomFish)}!\n" +
            $"{nameof(totalWeight)}:       {totalWeight}\n" +
            $"{nameof(weightedSelection)}: {weightedSelection}\n"
        );
    }
}
