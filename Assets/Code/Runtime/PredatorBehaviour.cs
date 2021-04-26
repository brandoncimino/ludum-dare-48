using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEngine;

public class PredatorBehaviour : FishBehaviour
{
    protected override void GotCaught(Catchables newCatch) {
        if (newCatch == this)
        {
            EventManager.Single.TriggerCollisionShark();
        }
    }
    
}
