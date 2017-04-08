using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeController : MonoBehaviour
{
    public GameObject   BlackholeVFX;
    public float        MoveSpeed = 50.0f;
    public float        LifeExpectancy = 5.0f;
    public float        GetTargetTime = 1.0f;
    public float        PlayerMultiplier = 0.2f;
    GameObject[] _charger;

    private GameObject[]    _asteroid;
    private GameObject      _player;
    private GameObject[]    _hunter;
    private GameObject      _blackhole;
    private GameManager     _gameManager;


	void Awake ()
    {
        this._blackhole = Instantiate(this.BlackholeVFX, this.transform.position, Quaternion.identity);
        this._gameManager = GameObject.Find("MainCamera").GetComponent<GameManager>();
    }

    void Start()
    {
        InvokeRepeating("GetTarget", 0, GetTargetTime);
        Die();
    }

    void Update ()
    {
        Aspire(this._player);
        Aspire(this._hunter);
        Aspire(this._asteroid);
        Aspire(this._charger);
	}

    void GetTarget()
    {
        this._player = GameObject.FindGameObjectWithTag("Player");
        this._hunter = GameObject.FindGameObjectsWithTag("Hunter");
        this._asteroid = GameObject.FindGameObjectsWithTag("Asteroid");
        this._charger = GameObject.FindGameObjectsWithTag("Charger");
    }

    void Die()
    {
        Destroy(this._blackhole.gameObject, LifeExpectancy);
        Destroy(this.gameObject, LifeExpectancy);
    }

    void Aspire(GameObject obj)
    {
        if(obj != null)
        {
            float distX = (obj.transform.position.x - this._blackhole.transform.position.x);
            float distZ = (obj.transform.position.z - this._blackhole.transform.position.z);
            float dist = distX * distX + distZ * distZ;
            float distanceMax = 10.0f * 10.0f;
            if (dist < distanceMax)
            {
                var power = this.MoveSpeed / (dist);
                var lookat = new Vector3(this._blackhole.transform.position.x - obj.transform.position.x, 0, this._blackhole.transform.position.z - obj.transform.position.z);
                lookat.Normalize();
                if (obj.tag == "Player")
                {
                    obj.transform.Translate(lookat * power * this.PlayerMultiplier * Time.deltaTime, Space.World);
                }
                else
                {
                    obj.transform.Translate(lookat * power * Time.deltaTime, Space.World);
                }
            }
        }
        
    }

    void Aspire(GameObject[] objs)
    {
        if(objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                Aspire(objs[i]);
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Player":
                other.gameObject.SetActive(false);
                this._gameManager.GameOverRequest();
                break;

            case "Hammer":
                break;

            case "Boundary":
                break;

            default:
                Destroy(other.gameObject);
                break;
        }
    }
}
