using System;
using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEngine;

public class DebrisBehaviour : Catchables
{
    // Start is called before the first frame update
    void Start()
    {
        // subscribe to the event manager
        EventManager.Single.ONTriggerCollisionDebris += gotCaught;
    }

    private void OnDestroy()
    {
        // unsubscribe to the event manager
        EventManager.Single.ONTriggerCollisionDebris -= gotCaught;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void gotCaught(DebrisBehaviour debris) {
        // just as a test behaviour to show that collision works

        if (debris == this)
        {
            transform.eulerAngles = 90f * Vector3.left;
        }
        
    }
}
