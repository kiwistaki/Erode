using System.Collections.Generic;
using Assets.Scripts.Camera;
using System.Linq;
using UnityEngine;

public class WindController : MonoBehaviour {

    public float MoveSpeed = 2.0f;

    private GameObject[] _asteroid;
    private GameObject _player;
    private GameObject[] _hunter;
    private GameObject[] _shooter;
    private GameObject _solarwind;
    private Dictionary<Vector3, float> _force;
    private Vector3 _direction;
    private CameraController _camera;

    void Start()
    {
        this._force = new Dictionary<Vector3, float>();
    }

    void Update()
    {
        this.ReduceTimer();
        this.CalculateDirection();

        if (this._force.Count > 0)
        {
            this.GetTarget();

            BlowAway(this._player);
            BlowAway(this._hunter);
            BlowAway(this._asteroid);
            BlowAway(this._shooter);
        }
    }

    public void AddVector(Vector3 direction, float duration)
    {
        this._force.Add(direction, duration);
    }

    public void ReduceTimer()
    {
        List<Vector3> keys = new List<Vector3>();
        for( int i = 0; i < this._force.Count; i++)
        {
            this._force[this._force.Keys.ElementAt(i)] = this._force[this._force.Keys.ElementAt(i)] - Time.deltaTime;
            if (this._force[this._force.Keys.ElementAt(i)] < 0.0f)
            {
                keys.Add(this._force.Keys.ElementAt(i));
            }   
        }

        foreach(var f in keys)
        {
            this._force.Remove(f);
        }

    }

    void CalculateDirection()
    {
        this._direction = new Vector3(0.0f, 0.0f, 0.0f);
        foreach(var f in this._force)
        {
           this._direction += f.Key;
        }
        //float norm = Mathf.Sqrt(this._direction.x* this._direction.x) + (this._direction.z* this._direction.z);
        this._direction.x = this._direction.x / this._force.Count;
        this._direction.y = 0.0f;
        this._direction.z = this._direction.z / this._force.Count;

    }

    void GetTarget()
    {
        this._player = GameObject.FindGameObjectWithTag("Player");
        this._hunter = GameObject.FindGameObjectsWithTag("Hunter");
        this._asteroid = GameObject.FindGameObjectsWithTag("Asteroid");
        this._shooter = GameObject.FindGameObjectsWithTag("Shooter");
    }


    void BlowAway(GameObject obj)
    {
        if (obj != null)
        {
            if (obj.tag == "Player")
            {
                obj.transform.Translate(this._direction * MoveSpeed * Time.deltaTime / 2, Space.World);
            }
            else
            {
                obj.transform.Translate(this._direction * MoveSpeed * Time.deltaTime, Space.World);
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
