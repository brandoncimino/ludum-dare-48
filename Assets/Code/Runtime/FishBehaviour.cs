using System;
using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEditor.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishBehaviour : MonoBehaviour
{
    public Collider myInnerCollider;
    public Rigidbody myRigidbody;

    protected float speed = 5;
    //protected Vector3 direction = Vector3.forward;
    
    protected bool isUncomfortable = false;

    protected float timeTillChange = 4f;
    protected float minTimeTillChange = 1f;
    protected float maxTimeTillChange = 7f;
    
    protected float timeForChange = 0.1f;
    protected float minTimeForChange = 0.2f;
    protected float maxTimeForChange = 1f;
    protected float AngleChange = 90;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // subscribe to the event manager
        EventManager.Single.ONTriggerCollisionFish += gotCaught;
        
        // adjust direction to own rotation
        //direction = transform.rotation * direction;
        timeTillChange = Random.Range(minTimeTillChange, maxTimeTillChange);
    }

    private void OnDestroy()
    {
        // cancel all substrictions to the event manager
        EventManager.Single.ONTriggerCollisionFish -= gotCaught;
    }
    
    // Update is called once per frame
    void Update()
    {
        // change direction if feeling uncomfortable
        if (isUncomfortable)
        {
            // always avoid in the right direction
            // var angle = 90 * Time.deltaTime;
            // transform.Rotate(transform.up, angle);
            //direction = Quaternion.Euler(angle * Vector3.up) * direction;
        }
        else
        {
            // changeDirectionAtRandom();
        }

        // move forward
        myRigidbody.velocity = speed * transform.forward;
    }

    public void Scare()
    {
        isUncomfortable = true;
    }

    public void CalmDown()
    {
        isUncomfortable = false;
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

    private void changeDirectionAtRandom()
    {
        
        timeTillChange -= Time.deltaTime;
        if (timeTillChange < 0)
        {
            timeForChange -= Time.deltaTime;
            
            transform.Rotate(transform.up, AngleChange * Time.deltaTime);
            //direction = Quaternion.Euler((AngleChange * Time.deltaTime) * Vector3.up) * direction;
            
            // start moving forward again when your change time is up
            if (timeForChange < 0)
            {
                timeTillChange = Random.Range(minTimeTillChange, maxTimeTillChange);
                timeForChange = Random.Range(minTimeForChange, maxTimeForChange);
                AngleChange = Random.Range(-90f, 90f);
            }
        }
    }
}
