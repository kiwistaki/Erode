using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarwindSpawner : MonoBehaviour {

    public GameObject Solarwind;
    public float SpawnTime = 10.0f;

    void Awake()
    {
        InvokeRepeating("Spawn", 0, SpawnTime);
    }

    void Spawn()
    {
        // Instantiation of new gameobject
        Instantiate(Solarwind, new Vector3(0, 0, 0), (Quaternion.Euler(0, 0, 0)));
    }
}
