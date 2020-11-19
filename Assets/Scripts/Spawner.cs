using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform spawnLocation;
    public GameObject boidManager;
    public Boid prefab;
    public int spawnCount = 10;
    public float spawnRadius = 10;
    public Color boidColor;

    void Awake()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = spawnLocation.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate  (prefab, boidManager.transform);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            boid.SetColour(boidColor);
        }
    }
}
