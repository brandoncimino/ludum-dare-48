using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    public Collider myInnerCollider;
    public Transform myComfortZone;
    public LayerMask fishMask;

    protected float speed = 5;
    protected Vector3 direction = Vector3.forward;
    protected bool isUncomfortable = false;
    protected float myComfortRadius = 2;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // subscribe to the event manager
        EventManager.Single.ONTriggerCollisionFish += gotCaught;
        
        // adjust direction to own rotation
        direction = transform.rotation * direction;
    }

    private void OnDestroy()
    {
        // cancel all substrictions to the event manager
        EventManager.Single.ONTriggerCollisionFish -= gotCaught;
    }
    
    // Update is called once per frame
    void Update()
    {
        // check if anything is inside the comfort zone
        isUncomfortable = Physics.CheckSphere(myComfortZone.position, myComfortRadius, fishMask);

        // change direction if feeling uncomfortable
        if (isUncomfortable)
        {
            var angle = 90f * Time.deltaTime;
            transform.Rotate(Vector3.up, angle);
            direction = Quaternion.Euler(angle * Vector3.up) * direction;
        }
        
        // move forward
        transform.localPosition += (speed * Time.deltaTime) * direction;
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
    }
    
}
