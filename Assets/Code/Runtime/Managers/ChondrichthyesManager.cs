using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;

using Code.Runtime;

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
    public float   distanceFromCameraCrewToSpawnFish;

    [Range(0, 100)]
    public float chanceToUseGoblinShark;

    private float fishScaleMin  = 0.1f;
    private float fishScaleStep = 0.3f;

    [Header("The weighted distribution used to select which fish will be spawned")]
    public List<Pair<Catchables, int>> Fishtrobution;

    /// A much simpler and probably smarter implementation of <see cref="Code.Runtime.Bathymetry.Holder"/>
    private static Lazy<Transform> FishHolder = new Lazy<Transform>(() => new GameObject(nameof(FishHolder)).transform);

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
        if (ShouldSpawn()) {
            SpawnFish();
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
    /// <returns></returns>
    private Catchables SpawnFish() {
        var fishToSpawn          = PickRandomFish();
        var spawnBenthicDistance = Random.value;
        var spawnBenthicBreadth  = Random.value;
        var spawnWorldPosition   = GameManager.Single.LazyCoaster.Value.BenthicPointToWorldPoint(spawnBenthicDistance, spawnBenthicBreadth);

        // fish have a different size as a motivation to go deep
        var fishScale = fishScaleMin + fishScaleStep * (gameLevel + Random.Range(-1f, 1f));

        var newFish = InstantiateFish(fishToSpawn, spawnWorldPosition, fishScale);

        GenerateFishTicket();

        return newFish;
    }

    /// <summary>
    /// Spawns a <a href="https://en.wikipedia.org/wiki/Clutch_(eggs)">clutch</a> of fish
    /// </summary>
    /// <param name="clutchSize">the size of the <a href="https://en.wikipedia.org/wiki/Clutch_(eggs)">clutch</a></param>
    /// <returns></returns>
    public List<Catchables> SpawnFish(int clutchSize) {
        return Enumerable.Repeat(SpawnFish(), clutchSize).ToList();
    }

    /// <summary>
    /// "Instantiates" a <see cref="Catchables"/> prefab.
    ///
    /// This method should not contain any "decision making", and should only handle the "engine-y" stuff.
    /// Decision making should go in <see cref="SpawnFish"/> instead.
    /// </summary>
    /// <param name="fishToInstantiate"></param>
    /// <param name="position"></param>
    /// <param name="fishScale"></param>
    /// <returns></returns>
    private static Catchables InstantiateFish(Catchables fishToInstantiate, Vector3 position, float fishScale) {
        var newFish = Instantiate(fishToInstantiate, position, Quaternion.identity, FishHolder.Value);
        newFish.ScaleUp(fishScale);
        return newFish;
    }

    private Catchables PickRandomFish() {
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
