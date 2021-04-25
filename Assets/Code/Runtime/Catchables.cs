using System;
using UnityEngine;

namespace Code.Runtime
{
    public class Catchables : MonoBehaviour
    {
        protected void Start()
        {
            // subscribe to the event manager
            EventManager.Single.ONTriggerCollisionCatchable += GotCaught;
            
            myStartBehaviour();
        }
        
        protected void OnDestroy()
        {
            // unsubscribe to the event manager
            EventManager.Single.ONTriggerCollisionCatchable -= GotCaught;
        }

        protected void GotCaught(Catchables newCatch)
        {
            if (newCatch == this)
            {
                myPersonalTrigger();
                
                // just to show that something is happening
                transform.eulerAngles = 90f * Vector3.left;
            }
        }

        protected virtual void myStartBehaviour()
        {
            // nothing has to happen
        }

        protected virtual void myPersonalTrigger()
        {
            // subclass dependent trigger
        }
    }
}