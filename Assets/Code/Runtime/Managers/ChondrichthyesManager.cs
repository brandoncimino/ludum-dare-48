using System;
using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class ChondrichthyesManager : MonoBehaviour
{
    public Camera cameraCrew;
    public Catchables goblinShark;
    public Catchables basicFish;

    public float minimumTimeBeforeNextFish;
    public float maximumTimeBeforeNextFish;
    public float distanceFromCameraCrewToSpawnFish;
    [Range(0, 100)] public float chanceToUseGoblinShark;

    private float fishScaleMin = 0.1f;
    private float fishScaleStep = 0.3f;

    public int gameLevel => GameManager.Single.lvl;

    private float lastFishTimeStamp;
    private float nextFishDelivery;
    
    // Start is called before the first frame update
    void Start()
    {
        lastFishTimeStamp = 0f;
        GenerateFishTicket();
        
        // subscribe to the event manager
    }

    private void OnDestroy()
    {
        // unsubscribe to the event manager
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
        var newFish = Instantiate(RandomFish(), ray.GetPoint(distanceFromCameraCrewToSpawnFish), Random.rotation);
        
        // fish have a different size as a motivation to go deep
        newFish.ScaleUp(fishScaleMin + fishScaleStep * (gameLevel + Random.Range(-1f, 1f)));
    }

    private Catchables RandomFish()
    {
        if (Random.Range(0, 100) < chanceToUseGoblinShark * (gameLevel - 1))
        {
            return goblinShark;
        }
        return basicFish;
    }

    
}
