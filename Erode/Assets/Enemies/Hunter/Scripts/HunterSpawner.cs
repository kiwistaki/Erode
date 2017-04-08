using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using Assets.Scripts.HexGridGenerator;
using UnityEngine;


public class HunterSpawner : MonoBehaviour {

    public float spawnTime = 8.0f;
    public GameObject hunter;
    public GameObject spawner;

    private Tile _tile;
    private Vector3 _pos;

    void Start()
    {
        InvokeRepeating("SpawnSpawner", 0, spawnTime);
        InvokeRepeating("SpawnHunter", 2, spawnTime);
    }

    void Update()
    {
    }

    void SpawnHunter()
    {
        Instantiate(hunter, this._pos, (Quaternion.Euler(0, 0, 0)));
    }

    void SpawnSpawner()
    {
        this._tile = Grid.inst.GetRandomTile(true, false);
        this._pos = this._tile.transform.position;
        Instantiate(spawner, new Vector3(this._pos.x, this._pos.y + 0.5f, this._pos.z), (Quaternion.Euler(0, 0, 0)));
    }
}
