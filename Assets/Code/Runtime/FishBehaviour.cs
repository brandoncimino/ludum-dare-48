using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{

    public float test = 0;

    private Collider myInnerCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        // subscribe to the event manager
        EventManager.Single.ONTriggerCollisionFish += gotCaught;
    }

    private void OnDestroy()
    {
        // cancel all substrictions to the event manager
        EventManager.Single.ONTriggerCollisionFish -= gotCaught;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        myInnerCollider.isTrigger = false;
        EventManager.Single.TriggerCollisionFish(this);
    }

    private void gotCaught(FishBehaviour fish)
    {
        // just as a test behaviour to show that collision works
        transform.eulerAngles = 90f * Vector3.left;
        test = 1f;
    }
    
}
