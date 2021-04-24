using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehaviour : MonoBehaviour
{
    public static HookBehaviour Single;
    public CharacterController controller;
    
    private float _depth;
    private float _pressure;
    private float _depthInitial;
    
    private Vector3 _velocityPull = new Vector3(0, 0, 0 );
    private Vector3 _velocityPush = new Vector3(0, 0, 0 );
    private float _pullModifier = 0.001f;
    private float _pushModifier = 0.001f;
    private float _maxDepth = 1000f;

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
        // TODO: David wanted to do this
        
        // movement in total (additive as an approximation)
        var moveVector = (_pullModifier * _velocityPull + _pushModifier * _velocityPush) * Time.deltaTime;
        controller.Move(moveVector);

    }

    private void CollisionFish()
    {
        // not implemented yet
    }
    
}
