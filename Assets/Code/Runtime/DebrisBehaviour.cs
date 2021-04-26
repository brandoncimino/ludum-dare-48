using System;
using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEngine;

public class DebrisBehaviour : Catchables
{
    
    protected override void myPersonalTrigger()
    {
        EventManager.Single.TriggerCollisionDebris(this);
    }
}
