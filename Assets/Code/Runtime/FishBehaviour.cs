using System.Transactions;
using Code.Runtime;
using JetBrains.Annotations;
using TMPro;

using UnityEngine;

public class FishBehaviour : Catchables {
    public Collider myInnerCollider;

    protected float speed = 5;
    protected float fleeAngle = 90f;
    protected float fleeTime = 1f;

    //protected Vector3 direction = Vector3.forward;

    public bool isUncomfortable = false;

    protected float timeTillChange    = 4f;
    protected float minTimeTillChange = 3f;
    protected float maxTimeTillChange = 7f;

    protected float timeForChange    = 0.1f;
    protected float minTimeForChange = 0.2f;
    protected float maxTimeForChange = 0.4f;
    protected float AngleChange      = 90;

    public TMP_Text thoughts;

    protected override void myStartBehaviour() {
        timeTillChange = Random.Range(minTimeTillChange, maxTimeTillChange);
        AngleChange = Random.Range(-90f, 90f);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        // change direction if feeling uncomfortable
        if (isUncomfortable)
        {
            fleeTime -= Time.deltaTime;
            if (fleeTime < 0)
            {
                CalmDown();
            }
            
            transform.Rotate(Vector3.up, fleeAngle * Time.deltaTime);
        }
        else {
            changeDirectionToHorizonntal();
            changeDirectionAtRandom();
        }

        // move forward
        catchableRigidbody.velocity = (speed + Mathf.Sin(timeForChange + timeTillChange)) * transform.forward;
    }


    public virtual void Scare([CanBeNull] Transform enemy = null) {
        isUncomfortable = true;
        thoughts.text   = "AAAAAH";
        fleeAngle = Random.Range(10f, 90f);
    }

    public virtual void CalmDown([CanBeNull] Transform enemy = null) {
        isUncomfortable = false;
        thoughts.text   = "that was close";

        fleeTime = Random.Range(1f, 2f);
        
        timeTillChange = Random.Range(minTimeTillChange, maxTimeTillChange);
        timeForChange  = Random.Range(minTimeForChange,  maxTimeForChange);
        AngleChange    = Random.Range(-90f,              90f);
    }


    protected void changeDirectionAtRandom() {
        // count down until you change directions again
        timeTillChange -= Time.deltaTime;
        if (!(timeTillChange < 0)) return;

        // rotate to your heart's desire
        timeForChange -= Time.deltaTime;
        transform.Rotate(Vector3.up, AngleChange * Time.deltaTime);
        if (!(timeForChange < 0)) return;

        // start moving forward again when your change time is up
        timeTillChange = Random.Range(minTimeTillChange, maxTimeTillChange);
        timeForChange  = Random.Range(minTimeForChange,  maxTimeForChange);
        AngleChange    = Random.Range(-90f,              90f);
    }

    protected void changeDirectionToHorizonntal() {
        var horizontalRotation = new Quaternion();
        horizontalRotation.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation             = Quaternion.RotateTowards(transform.rotation, horizontalRotation, 20 * Time.deltaTime);
    }

    protected override void myPersonalTrigger() {
        EventManager.Single.TriggerCollisionFish(this);
    }
}
