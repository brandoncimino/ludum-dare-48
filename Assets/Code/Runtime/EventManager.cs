using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Single;

    private void Awake()
    {
        Single = this;
    }

    public event Action <FishBehaviour> ONTriggerCollisionFish;
    public void TriggerCollisionFish(FishBehaviour fish)
    {
        ONTriggerCollisionFish?.Invoke(fish);
    }

    public event Action ONTriggerFirstFishCaught;

    public void TriggerFirstFishCaught()
    {
        ONTriggerFirstFishCaught();
    }

}
