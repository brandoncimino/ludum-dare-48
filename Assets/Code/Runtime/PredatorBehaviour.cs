using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEngine;

public class PredatorBehaviour : FishBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    protected override void GotCaught(Catchables newCatch) {
        if (newCatch == this)
        {
            EventManager.Single.TriggerCollisionShark();
        }
    }
}
