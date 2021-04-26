using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEngine;

public class FishCatcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Catchables>() != null) {
            EventManager.Single.TriggerCollisionCatchable(other.GetComponent<Catchables>());
        }
    }
}
