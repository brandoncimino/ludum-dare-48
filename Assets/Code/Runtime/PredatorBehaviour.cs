using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BrandonUtils.Standalone.Collections;
using Code.Runtime;
using JetBrains.Annotations;
using UnityEngine;

public class PredatorBehaviour : FishBehaviour
{
    public bool isChasing = false;
    public List<Transform> targets = new List<Transform>();
    public float sharktest = 0;

    protected override void Update()
    {
        var tempSpeed = speed;
        
        if (targets.Count > 0) {
            
            isChasing = true;
            var target = targets.Random();
            if (!target.gameObject.activeSelf)
            {
                targets.Remove(target);
            }
            
            var targetDirection = target.position - transform.position;
            var newDirection = Vector3.RotateTowards(transform.forward, targetDirection,
                ((2 * Mathf.PI * 20f) / 180) * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            tempSpeed = 2f * speed;
        }
        else {
            changeDirectionToHorizonntal();
            changeDirectionAtRandom();
            
        }

        // move forward
        catchableRigidbody.velocity = tempSpeed * transform.forward;
    }
    
    protected override void GotCaught(Catchables newCatch) {
        if (newCatch == this)
        {
            EventManager.Single.TriggerCollisionShark();
        }
    }
    
    public override void Scare([CanBeNull] Transform enemy = null) {
        if (!(enemy == null))
        {
            if (targets.Contains(enemy)) return;
            targets.Add(enemy);
            thoughts.text   = "oh, hello";
        }

    }

    public override void CalmDown([CanBeNull] Transform enemy = null)
    {
        if (!(enemy == null))
        {
            targets.Remove(enemy);
            thoughts.text = "see ya";
        }
    }
    
    public override void ScaleUp(float newScale)
    {
        myScale = Mathf.Max(newScale, 1f);
        transform.localScale = myScale * Vector3.one;
    }

}
