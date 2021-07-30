using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public Boid[] boids;
    public BoidSettings settings;
    public Boid.Steering_Behaviour behaviourType;
    public Transform target;

    public List<Transform> pathTargets;
    public GameObject path;

    public bool targeting = false;
    public bool active = true;

    public void TogglePauseBoids()
    {
        active = !active;
    }

    public void RefreshBoids()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids)
        {
            b.Initialize(settings, target);
        }
    }

    void Start()
    {
        RefreshBoids();
    }
    void Update()
    {
        if (!active) return;
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
            else if (behaviourType == Boid.Steering_Behaviour.PathGrapplingHooks)
            {
                Boid current = boids[i];
                for (int index = 0; index < boids.Length; index++)
                {
                    float distance = (current.position - pathTargets[current.pathIndex].transform.position).magnitude;
                    if (distance < current.settings.pathArriveDistance)
                    {
                        current.pathIndex++;
                        if (current.pathIndex > pathTargets.Count - 1) current.pathIndex = 0;
                    }
                    current.pathTarget = pathTargets[current.pathIndex];
                }
            }
            boids[i].UpdateBoid(behaviourType, targeting);
        }
    }

    public void ResetBoids()
    {
        foreach (Boid boid in boids)
        {
            DestroyImmediate(boid.gameObject);
        }
        RefreshBoids();
        Debug.Log(boids.Length);
    }

    public void ChangeType(int val)
    {
        path.SetActive(false);
        switch (val)
        {
            case 0:
                behaviourType = Boid.Steering_Behaviour.Flocking;
                break;
            case 1:
                behaviourType = Boid.Steering_Behaviour.PathGrapplingHooks;
                path.SetActive(true);
                break;
            case 2:
                behaviourType = Boid.Steering_Behaviour.DynamicWander;
                break;
            case 3:
                behaviourType = Boid.Steering_Behaviour.DynamicArrive;
                break;
            case 4:
                behaviourType = Boid.Steering_Behaviour.DynamicSeek;
                break;

            default: break;
        }
    }
}
