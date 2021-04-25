using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehaviour : MonoBehaviour
{

    public static HookBehaviour Single;
    public CharacterController controller;
    public Rigidbody MyRigidbody;
    
    private float _depth;
    private float _pressure;
    private float _depthInitial;
    
    private Vector3 _velocityPull = new Vector3(0, 0, 0 );
    private Vector3 _velocityPush = new Vector3(0, 0, 0 );
    public Vector3 _directionPush = new Vector3(0, 0, 0 );
    private float _pullModifier = 0.001f;
    private float _pushModifier = 0.001f;
    private float _maxDepth = 1000f;

    private List<FishBehaviour> myCatches;

    private void Awake()
    {
        Single = this;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        _depthInitial = transform.localPosition.y;
        
        // substrictions to the event manager
        EventManager.Single.ONTriggerCollisionFish += CollisionFish;
    }

    private void OnDestroy()
    {
        // cancel all substrictions to the event manager
        EventManager.Single.ONTriggerCollisionFish -= CollisionFish;
    }


    // Update is called once per frame
    void Update()
    {
        
        // falling down based on pressure and gravity
        _depth = Mathf.Abs(transform.localPosition.y - _depthInitial);
        _velocityPull.y = WaterManager.Single.computePull(_depth, _maxDepth);
        
        // user induced movement
        // var horizontal = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        // var horizontal = -Input.GetAxisRaw("Horizontal");
        // var vertical = -Input.GetAxisRaw("Vertical");
        //  horizontal = 1f;
        // _directionPush = new Vector3(horizontal, 0f, vertical);
        
        // movement in total (additive as an approximation)
        var moveVector = (_pullModifier * _velocityPull + _pushModifier * _velocityPush);
        MyRigidbody.velocity = moveVector;
        // transform.Translate(moveVector);
        //controller.Move(moveVector);

    }

    private void CollisionFish(FishBehaviour fish)
    {
        if (myCatches.Count == 0)
        {
            EventManager.Single.TriggerFirstFishCaught();
        }
        myCatches.Add(fish);
        // not implemented yet
    }
    
}
