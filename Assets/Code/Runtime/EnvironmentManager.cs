using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    //Potential variables for generating course during gameplay
    public Camera mainCamera;
    public int courseWidth;
    public int courseHeight;
    public Vector2 courseDiameter;
    public float courseRadius;
    public int courseDepth;

    public GameObject tidalRock1;
    public GameObject tidalRock2;
    public GameObject tubeWeed;
    public GameObject acornBarnacle;
    public GameObject coral;
    public GameObject wall;

    [Range(0,100)]
    public int terrainWallDensityPercentage;
    [Range(0,100)]
    public int terrainWallUseLargeRockPercentage;
    [Range(0,100)]
    public float terrainWallPositionRandomizationPercentage;
    [Range(0.1f,10)]
    public float terrainWallBaseSpaceBetweenRocks;
    [Range(0,1)]
    public float terrainWallRockSizeRandomizationPercentage;

    [Range(0, 10)] public float scalingFactor = 10f;

    public int numberOfRocksPerRing;

    private Transform environmentHolder;
    
    // Start is called before the first frame update
    void Start()
    {
        environmentHolder = new GameObject(nameof(environmentHolder)).transform;
        InstantiateTerrain();
        
        // scale up so we can move around
        environmentHolder.localScale = scalingFactor * Vector3.one;
    }

    private void InstantiateTerrain()
    {
        //Generate 4 walls based on the given course Height and Width
        //NorthWall
        //InstantiateTerrainHorizontalWall(new Vector3(courseWidth / 2 * -1,0,courseHeight / 2),courseWidth);
        //SouthWall
        //InstantiateTerrainHorizontalWall(new Vector3(courseWidth / 2 * -1, 0, courseHeight / 2 * -1), courseWidth);
        //EastWall
        //InstantiateTerrainVerticalWall(new Vector3(courseWidth / 2 * -1, 0, courseHeight / 2 * -1), courseHeight);
        //WestWall
        //InstantiateTerrainVerticalWall(new Vector3(courseWidth / 2, 0, courseHeight / 2 * -1), courseHeight);
        
        InstantiateRockTube();
        
        
    }

    private void InstantiateRockTube()
    {
        for (int y = 0; y < courseDepth; y++)
        {
            InstantiateRockRing(-y);
        }
    }

    private void InstantiateRockRadially(float depth, float degrees)
    {
        if (Random.Range(0, 100) > terrainWallDensityPercentage) return;
        var lateralPosition = Rotate(Vector2.right * courseRadius, degrees);
        MakeRock(lateralPosition, depth);
    }

    private void InstantiateRockRing(float depth)
    {
        for (int r = 0; r < numberOfRocksPerRing; r++)
        {
            InstantiateRockRadially(depth, 360/numberOfRocksPerRing * (r+1));
        }
    }

    //Rotates a point around 0
    private static Vector2 Rotate(Vector2 vector2, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * vector2;
    }

    private void MakeRock(Vector2 lateralPosition, float depth)
    {
        Instantiate(RandomRock(), RockLocationWithNoise(lateralPosition,depth), Random.rotation,
            environmentHolder);
    }

    private Vector3 RockLocationWithNoise(Vector2 lateralPosition, float depth)
    {
        var xResult = lateralPosition.x *= 1+Random.Range(-terrainWallPositionRandomizationPercentage,
            terrainWallPositionRandomizationPercentage);
        var yResult = depth *= 1+Random.Range(-terrainWallPositionRandomizationPercentage,
            terrainWallPositionRandomizationPercentage);
        var zResult = lateralPosition.y *= 1+Random.Range(-terrainWallPositionRandomizationPercentage,
            terrainWallPositionRandomizationPercentage);
        return new Vector3(xResult, yResult, zResult);
    }

    private void InstantiateTerrainVerticalWall(Vector3 startPoint, int wallLength)
    {
        for (float y = 0; y > courseDepth * -1; y-=terrainWallBaseSpaceBetweenRocks)
        {
            for (float z = 0; z < wallLength; z+=terrainWallBaseSpaceBetweenRocks)
            {
                if (Random.Range(0, 100) < terrainWallDensityPercentage)
                {
                    Instantiate(RandomRock(), new Vector3(startPoint.x, startPoint.y + y, startPoint.z+z),
                        Random.rotation);
                }
                
            }
        }
    }

    private void InstantiateTerrainHorizontalWall(Vector3 startPoint, int wallLength)
    {
        for (float y = 0; y > courseDepth * -1; y-=terrainWallBaseSpaceBetweenRocks)
        {
            for (float x = 0; x < wallLength; x+=terrainWallBaseSpaceBetweenRocks)
            {
                if (Random.Range(0, 100) < terrainWallDensityPercentage)
                {
                    var generatedRock = Instantiate(RandomRock(), new Vector3(startPoint.x + x, startPoint.y + y, startPoint.z),
                        Random.rotation,environmentHolder);
                    //generatedRock.transform *= Vector3.one * terrainWallRockSizeRandomizationPercentage;
                }
                
            }
        }
    }

    private GameObject RandomRock()
    {
        if (Random.Range(0, 100) < terrainWallUseLargeRockPercentage)
        {
            return tidalRock2;
        }
        return tidalRock1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
