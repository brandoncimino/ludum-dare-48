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
        // this means 10000 N m^(-3)
        // this means 10000 km s^{-2} m^{-2}
        return 1e4f * Mathf.Abs(depth) + pressureSurface;
    }

    public float computeVelocityConst(float maxDepth)
    {
        return (1e4f * maxDepth + gravity * maxDepth) / density;
    }
    public float computePull(float depth, float maxDepth = 1e4f)
    {
        // fall velocity according to Bernoulli principle
        var velocity = computeVelocityConst(maxDepth);
        velocity += (pressureSurface - computePressure(depth)) / density;
        velocity -= gravity * Mathf.Abs(depth);
        
        // not sure how to decide on the sign
        var sign = Mathf.Sign(velocity);
        velocity = -sign * Mathf.Sqrt(2 * sign * velocity);
        return velocity;
    }
}
