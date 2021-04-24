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

    public event Action ONTriggerCollisionFish;

    public void TriggerCollisionFish()
    {
        ONTriggerCollisionFish?.Invoke();
    }


}
