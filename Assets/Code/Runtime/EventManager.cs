using System;
using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
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

    public event Action ONTriggerFirstCatch;

    public void TriggerFirstCatch()
    {
        ONTriggerFirstCatch();
    }
    
    public event Action <DebrisBehaviour> ONTriggerCollisionDebris;
    public void TriggerCollisionDebris(DebrisBehaviour debris)
    {
        ONTriggerCollisionDebris?.Invoke(debris);
    }
    
    
    public event Action <Catchables> ONTriggerCollisionCatchable;
    public void TriggerCollisionCatchable(Catchables newCatch)
    {
        ONTriggerCollisionCatchable?.Invoke(newCatch);
    }

}
