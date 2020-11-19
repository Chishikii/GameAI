using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    //Settings
    public float maxSpeed = 5f;
    public float maxSteerForce = 5f;
    public float arriveSlowRadius = 5f;
    public float avoidanceRadius = 1;

    public float viewRadius = 10f;
    public float perceptionRadius = 2f;

    [Header("Weights")]
    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float seperateWeight = 1;

    [Header("Wander")]
    public float wanderCircleOffset = 7f;
    public float wanderCircleRadius = 4f;

    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;
}
