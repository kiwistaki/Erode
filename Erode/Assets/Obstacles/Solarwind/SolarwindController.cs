using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Camera;
using UnityEngine;

public class SolarwindController : MonoBehaviour {

    public GameObject   SolarwindVFX;
    public float        MoveSpeed = 2.0f;
    public float        LifeExpectancy = 5.0f;
    public float        GetTargetTime = 1.0f;
    public float        PlayerMultiplier = 0.4f;
    public float        Smoothing = 5f;

    private GameObject[]        _asteroid;
    private GameObject          _player;
    private GameObject[]        _hunter;
    private GameObject[]        _shooter;
    private GameObject          _solarwind;
    private Vector3             _direction;
    private CameraController    _camera;


    void Awake()
    {
        this._direction = new Vector3(Random.Range(-1.0f,1.0f), 0, Random.Range(-1.0f, 1.0f));
        this._camera = GameObject.Find("MainCamera").GetComponent<CameraController>();
        this._solarwind = Instantiate(this.SolarwindVFX, new Vector3(this._camera.transform.position.x, this._camera.transform.position.y/2, this._camera.transform.position.z), Quaternion.identity);
    }

    void Start()
    {
        InvokeRepeating("GetTarget", 0, GetTargetTime);
        Die();
    }

    void Update()
    {
        this._solarwind.transform.position = Vector3.Lerp(new Vector3(this._camera.transform.position.x, this._camera.transform.position.y / 2, this._camera.transform.position.z), this._solarwind.transform.position, Smoothing * Time.deltaTime);
        BlowAway(this._player);
        BlowAway(this._hunter);
        BlowAway(this._asteroid);
        BlowAway(this._shooter);
    }

    void GetTarget()
    {
        this._player = GameObject.FindGameObjectWithTag("Player");
        this._hunter = GameObject.FindGameObjectsWithTag("Hunter");
        this._asteroid = GameObject.FindGameObjectsWithTag("Asteroid");
        this._shooter = GameObject.FindGameObjectsWithTag("Shooter");
    }

    void Die()
    {
        Destroy(this._solarwind.gameObject, LifeExpectancy);
        Destroy(this.gameObject, LifeExpectancy);
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
                obj.transform.Translate((obj.transform.position + this._direction) * MoveSpeed * (Time.deltaTime/4), Space.World);
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
