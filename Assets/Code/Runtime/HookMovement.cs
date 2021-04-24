using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookMovement : MonoBehaviour
{
    public HookMovement Single;
    public CharacterController controller;
    
    private float _depth;
    private float _pressure;
    private float _depthInitial;
    
    private Vector3 _velocityPull = new Vector3(0, 0, 0 );
    private Vector3 _velocityPush = new Vector3(0, 0, 0 );
    private float _pullModifier = 0.01f;
    private float _pushModifier = 1f;

    private void Awake()
    {
        Single = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _depthInitial = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        _depth = Mathf.Abs(transform.localPosition.y - _depthInitial);
        _velocityPull.y = WaterManager.Single.computePull(_depth);
        var moveVector = (_pullModifier * _velocityPull + _pushModifier * _velocityPush) * Time.deltaTime;
        controller.Move(moveVector);

    }
    
    
}
