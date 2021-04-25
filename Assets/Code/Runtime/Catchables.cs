using System;
using UnityEngine;

namespace Code.Runtime
{
    public class Catchables : MonoBehaviour
    {

        public CharacterJoint myMouth;
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
                HookUp();
                myPersonalTrigger();
                
                // just to show that something is happening
                //transform.eulerAngles = 90f * Vector3.left;
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

        protected virtual void HookUp()
        {
            myMouth.connectedBody = HookBehaviour.Single.FindHook();
        }
    }
}