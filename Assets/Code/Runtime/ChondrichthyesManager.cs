using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ChondrichthyesManager : MonoBehaviour
{
    public Camera cameraCrew;
    public GameObject goblinShark;
    public GameObject basicFish;

    public float minimumTimeBeforeNextFish;
    public float maximumTimeBeforeNextFish;
    public float distanceFromCameraCrewToSpawnFish;
    [Range(0, 100)] public float chanceToUseGoblinShark;

    public int gameLevel;

    private float lastFishTimeStamp;
    private float nextFishDelivery;
    
    // Start is called before the first frame update
    void Start()
    {
        lastFishTimeStamp = 0f;
        GenerateFishTicket();
    }

    /// <summary>
    /// Determine when the next fish will be spawned in
    /// </summary>
    private void GenerateFishTicket()
    {
        nextFishDelivery = lastFishTimeStamp + Random.Range(minimumTimeBeforeNextFish, maximumTimeBeforeNextFish);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextFishDelivery)
        {
            lastFishTimeStamp = Time.time;
            GenerateFishTicket();
            SpawnFish();
        }
    }

    /// <summary>
    /// Given camera position and game level, determine location ahead of hook and instantiate fish
    /// </summary>
    private void SpawnFish()
    {
        var ray = cameraCrew.ScreenPointToRay(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0));
        Instantiate(RandomFish(), ray.GetPoint(distanceFromCameraCrewToSpawnFish), Random.rotation);
    }

    private GameObject RandomFish()
    {
        if (gameLevel >= 1 && Random.Range(0, 100) <= chanceToUseGoblinShark)
        {
            return goblinShark;
        }
        return basicFish;
    }
}
