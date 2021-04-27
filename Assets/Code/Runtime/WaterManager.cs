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
        // in water, pressure changes by 1 bar per 10 m
        // = 10000 Pa m^{-2}
        // = 10000 N m^{-3}
        // = 10000 km s^{-2} m^{-2}
        var pressure = 1e4f * depth + pressureSurface;
        UIManager.Single.provideData("pressure", (Mathf.Round(pressure * 1e-5f)));
        return pressure;
    }

    public float computeVelocityConst(float maxDepth)
    {
        // rearrange Bernoullil's equation to get squared initial velocity such that the equilibrium is reached at depth = maxDepth
        return (1e4f * maxDepth + gravity * maxDepth) / density;
    }
    public float computePull(float depth, float maxDepth = 1e4f)
    {
        // fall velocity according to Bernoulli principle
        var velocity = computeVelocityConst(maxDepth);
        velocity += (pressureSurface - computePressure(depth)) / density;
        velocity -= gravity * Mathf.Abs(depth);
        
        // using the sign to make sure everything is well-defined and we get the right direction
        // (moving up and down at equilibrium because of the discontinuity in time of the Update function)
        var sign = Mathf.Sign(velocity);
        velocity = -sign * Mathf.Sqrt(2 * sign * velocity);
        
        return velocity;
    }
}
