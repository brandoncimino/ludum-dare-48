using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComfortZone : MonoBehaviour
{
    public FishBehaviour myFish;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        myFish.Scare(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        myFish.CalmDown(other.transform);
    }
}
