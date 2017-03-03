using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarwindController : MonoBehaviour {

    public GameObject SolarwindVFX;
    public float MoveSpeed = 2.0f;
    public float _LifeExpectancy = 5.0f;
    public float GetTargetTime = 1.0f;
    public float PlayerMultiplier = 0.4f;
    GameObject[] _asteroid;
    GameObject _player;
    GameObject[] _hunter;
    GameObject _solarwind;
    Vector3 _direction;


    void Awake()
    {
        this._direction = new Vector3(Random.Range(-1.0f,1.0f), 0, Random.Range(-1.0f, 1.0f));
        //this._solarwind = Instantiate(this.SolarwindVFX, this.transform.position, Quaternion.identity);
    }

    void Start()
    {
        InvokeRepeating("GetTarget", 0, GetTargetTime);
        Die();
    }

    void Update()
    {
        BlowAway(this._player);
        BlowAway(this._hunter);
        BlowAway(this._asteroid);
    }

    void GetTarget()
    {
        this._player = GameObject.FindGameObjectWithTag("Player");
        this._hunter = GameObject.FindGameObjectsWithTag("Hunter");
        this._asteroid = GameObject.FindGameObjectsWithTag("Asteroid");
    }

    void Die()
    {
        //Destroy(this._solarwind.gameObject, _LifeExpectancy);
        Destroy(this.gameObject, _LifeExpectancy);
    }

    void BlowAway(GameObject obj)
    {
        if (obj != null)
        {
            if (obj.tag == "Player")
            {
                obj.transform.Translate((obj.transform.position + this._direction) * (MoveSpeed / 4) * (Time.deltaTime / 2), Space.World);
            }
            else
            {
                obj.transform.Translate((obj.transform.position + this._direction) * MoveSpeed * Time.deltaTime, Space.World);
            }
        }

    }

    void BlowAway(GameObject[] objs)
    {
        if (objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                BlowAway(objs[i]);
            }
        }

    }
}
