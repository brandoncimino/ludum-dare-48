using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    
    public static WaterManager Single;
    
    public float gravity = -9.81f;                  // m / s^2
    public float pressureSurface = 1e5f;       // Pascal
    public float density = 1.023f;           // kg / litre
    
    private void Awake()
    {
        Single = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public float computePressure(float depth)
    {
        // pressure changes by 1 bar per 10 m
        // this means 10000 Pa per m
        return 1e4f * depth;
    }

    public float computePull(float depth)
    {
        // fall velocity according to Bernoulli principle
        var velocity = (pressureSurface + computePressure(depth)) / density;
        velocity -= gravity * depth;
        velocity = -Mathf.Sqrt(2 * velocity);
        return velocity;
    }
}
