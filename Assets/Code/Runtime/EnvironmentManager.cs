using UnityEngine;

public class EnvironmentManager : MonoBehaviour {
    //Potential variables for generating course during gameplay
    public float courseRadius;
    public int   courseDepth;

    public GameObject tidalRock1;
    public GameObject tidalRock2;
    public GameObject tubeWeed;
    public GameObject acornBarnacle;
    public GameObject coral;
    public GameObject wall;

    [Range(0, 100)]
    public int terrainWallDensityPercentage;
    [Range(0, 100)]
    public int terrainWallUseLargeRockPercentage;
    [Range(0, 100)]
    public float terrainWallPositionRandomizationPercentage;
    [Range(0.1f, 10)]
    public float terrainWallBaseSpaceBetweenRocks;
    [Range(0, 1)]
    public float terrainWallRockSizeRandomizationPercentage;

    public int numberOfRocksPerRing;

    private Transform environmentHolder;

    // Start is called before the first frame update
    void Start() {
        environmentHolder = new GameObject(nameof(environmentHolder)).transform;
        InstantiateTerrain();
    }

    private void InstantiateTerrain() {
        InstantiateRockTube();
    }

    private void InstantiateRockTube() {
        for (int y = 0; y < courseDepth; y++) {
            InstantiateRockRing(-y);
        }
    }

    private void InstantiateRockRadially(float depth, float degrees) {
        if (Random.Range(0, 100) > terrainWallDensityPercentage) {
            return;
        }

        var lateralPosition = Rotate(Vector2.right * courseRadius, degrees);
        MakeRock(lateralPosition, depth);
    }

    private void InstantiateRockRing(float depth) {
        for (int r = 0; r < numberOfRocksPerRing; r++) {
            InstantiateRockRadially(depth, 360 / numberOfRocksPerRing * (r + 1));
        }
    }

    //Rotates a point around 0
    private static Vector2 Rotate(Vector2 vector2, float degrees) {
        return Quaternion.Euler(0, 0, degrees) * vector2;
    }

    private void MakeRock(Vector2 lateralPosition, float depth) {
        Instantiate(
            RandomRock(),
            RockLocationWithNoise(lateralPosition, depth),
            Random.rotation,
            environmentHolder
        );
    }

    private Vector3 RockLocationWithNoise(Vector2 lateralPosition, float depth) {
        var xResult = lateralPosition.x *= 1 + Random.Range(
                                               -terrainWallPositionRandomizationPercentage,
                                               terrainWallPositionRandomizationPercentage
                                           );
        var yResult = depth *= 1 + Random.Range(
                                   -terrainWallPositionRandomizationPercentage,
                                   terrainWallPositionRandomizationPercentage
                               );
        var zResult = lateralPosition.y *= 1 + Random.Range(
                                               -terrainWallPositionRandomizationPercentage,
                                               terrainWallPositionRandomizationPercentage
                                           );
        return new Vector3(xResult, yResult, zResult);
    }

    private GameObject RandomRock() {
        if (Random.Range(0, 100) < terrainWallUseLargeRockPercentage) {
            return tidalRock2;
        }

        return tidalRock1;
    }
}
