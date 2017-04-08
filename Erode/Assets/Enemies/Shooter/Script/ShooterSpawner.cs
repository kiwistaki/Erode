using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HexGridGenerator;
using UnityEngine;

public class ShooterSpawner : MonoBehaviour {

    public float spawnTime = 10.0f;
    public GameObject shooter;
    public GameObject spawner;

    private Tile _tile;
    private Vector3 _pos;

    void Start()
    {
        InvokeRepeating("SpawnSpawner", 0, spawnTime);
        InvokeRepeating("SpawnShooter", 2, spawnTime);
    }

    void Update()
    {
    }

    void SpawnShooter()
    {
        Instantiate(shooter, this._pos, (Quaternion.Euler(0, 0, 0)));
    }

    void SpawnSpawner()
    {
        this._tile = Grid.inst.GetRandomTile(true, false);
        this._pos = this._tile.transform.position;
        Instantiate(spawner, new Vector3(this._pos.x, this._pos.y + 0.5f, this._pos.z), (Quaternion.Euler(0, 0, 0)));
    }
}
