using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEngine;

public class BigBite : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Catchables>() != null)
        {
            other.GetComponent<Catchables>().getEaten();
        }
        
        if (other.GetComponent<HookBehaviour>() != null)
        {
            other.GetComponent<HookBehaviour>().getEaten();
            EventManager.Single.TriggerGameOverFail();
        }
        
        if (other.GetComponent<HookBehaviourSubmarine>() != null)
        {
            other.GetComponent<HookBehaviourSubmarine>().getEaten();
            EventManager.Single.TriggerGameOverFail();
        }
    }
}
