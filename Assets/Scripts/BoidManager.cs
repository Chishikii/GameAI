using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    Boid[] boids;
    public BoidSettings settings;
    public Boid.Steering_Behaviour behaviourType;
    public Transform target;

    void Start()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids)
        {
            b.Initialize(settings, target);
        }
    }

    void Update()
    {
        for (int i = 0; i < boids.Length; i++)
        {
            if (behaviourType == Boid.Steering_Behaviour.Flocking)
            {
                Boid current = boids[i];
                current.avgFlockHeading = Vector3.zero;
                current.centreOfFlockmates = Vector3.zero;
                current.avgAvoidanceHeading = Vector3.zero;
                current.numPerceivedFlockmates = 0;

                for (int index = 0; index < boids.Length; index++)
                {
                    Vector3 offset = boids[index].position - current.position;
                    float distance = offset.magnitude;

                    if (distance <= current.settings.perceptionRadius)
                    {
                        current.numPerceivedFlockmates += 1;
                        current.avgFlockHeading += boids[index].velocity;
                        current.centreOfFlockmates += boids[index].position;

                        if (distance < current.settings.avoidanceRadius)
                        {
                            current.avgAvoidanceHeading -= offset.normalized;
                        }
                    }
                }
            }
            boids[i].UpdateBoid(behaviourType);
        }
    }
}
