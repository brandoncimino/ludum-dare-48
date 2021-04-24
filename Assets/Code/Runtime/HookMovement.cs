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
    
    public Vector3 _velocityPull = new Vector3(0, 0, 0 );
    public Vector3 _velocityPush = new Vector3(0, 0, 0 );
    public float _pullModifier = 1f;
    public float _pushModifier = 1f;
    private float _maxDepth = 1000f;

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
        _velocityPull.y = WaterManager.Single.computePull(_depth, _maxDepth);
        
        var moveVector = (_pullModifier * _velocityPull + _pushModifier * _velocityPush) * Time.deltaTime;
        controller.Move(moveVector);

    }
    
    
}
