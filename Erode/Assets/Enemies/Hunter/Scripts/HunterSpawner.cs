using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using Assets.Scripts.HexGridGenerator;
using UnityEngine;


public class HunterSpawner : MonoBehaviour {

    public float spawnTime = 8.0f;
    public GameObject hunter;



	void Start ()
    {
        InvokeRepeating("Spawn", 0, spawnTime);
    }
	
	void Update ()
    { 
	}

    void Spawn()
    {
        Tile tile = Grid.inst.GetRandomTile(true, false);
        Vector3 pos = tile.transform.position;
        GameObject newHunter = Instantiate(hunter, pos, (Quaternion.Euler(0, 0, 0)));
    }
}
