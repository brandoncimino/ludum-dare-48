using Code.Runtime;
using JetBrains.Annotations;
using TMPro;

using UnityEngine;

public class FishBehaviour : Catchables {
    public Collider myInnerCollider;

    protected float speed = 5;

    //protected Vector3 direction = Vector3.forward;

    public bool isUncomfortable = false;

    protected float timeTillChange    = 4f;
    protected float minTimeTillChange = 1f;
    protected float maxTimeTillChange = 7f;

    protected float timeForChange    = 0.1f;
    protected float minTimeForChange = 0.2f;
    protected float maxTimeForChange = 1f;
    protected float AngleChange      = 90;

    public TMP_Text thoughts;

    protected override void myStartBehaviour() {
        timeTillChange = Random.Range(minTimeTillChange, maxTimeTillChange);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // change direction if feeling uncomfortable
        if (isUncomfortable) {
            // always avoid in the right direction
            var angle = 90 * Time.deltaTime;
            transform.Rotate(Vector3.up, angle);
        }
        else {
            changeDirectionToHorizonntal();
            changeDirectionAtRandom();
        }

        // move forward
        catchableRigidbody.velocity = speed * transform.forward;
    }


    public virtual void Scare([CanBeNull] Transform enemy = null) {
        isUncomfortable = true;
        thoughts.text   = "AAAAAAAAAAAAAAAAAAAAAAAAAH";
    }

    public virtual void CalmDown([CanBeNull] Transform enemy = null) {
        isUncomfortable = false;
        thoughts.text   = "that was close";
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
