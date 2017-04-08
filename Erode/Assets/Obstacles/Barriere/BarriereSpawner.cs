using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarriereSpawner : MonoBehaviour {

	// Use this for initialization

    public float SpawnTime = 3.0f;
    public GameObject Barriere;
    public float Radius = 35.0f;
    [Range(0.0f, 180.0f)]
    public float AngleVariation = 35.0f;
    public float BarriereHeight = 1.5f;
    [Range(0.0f, 10.0f)]
    public float BarriereSpeed = 4.0f;
    [Range(0.0f, 180.0f)]
    public float BarriereAngularSpeed = 5.0f;
    public float MinBarriereAngularSpeed = 2.0f;

    private float _minDirectionAngle;
    private float _maxDirectionAngle;



    void Awake()
    {
        this._minDirectionAngle = (180.0f - this.AngleVariation) / 2.0f + 90.0f;
        this._maxDirectionAngle = 270.0f - (180.0f - this.AngleVariation) / 2.0f;
    }

    void Start()
    {
        if (this._minDirectionAngle > this._maxDirectionAngle)
        {
            var tmp = this._maxDirectionAngle;
            this._maxDirectionAngle = this._minDirectionAngle;
            this._minDirectionAngle = tmp;
        }
        this.InvokeRepeating("Spawn", 0, this.SpawnTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //La barrière va spawn a un angle random et une distance fixe de la plateforme
    void Spawn()
    {
        //L'angle utilisé pour faire spawner la barriere va être random, ainsi que la rotation la barriere
        float spawnAngleSource = Random.Range(0, 359);
        //Si l'angle entre les deux points est très basse, les chances la barriere passe au dessus de la plateforme est mince.
        float spawnAngleDest = Random.Range(this._minDirectionAngle, this._maxDirectionAngle);
        if ((spawnAngleDest += spawnAngleSource) > 360)
        {
            spawnAngleDest -= 360;
        }

        Vector3 startPos = this.CalculatePosition(spawnAngleSource);
        Vector3 endPos = this.CalculatePosition(spawnAngleDest);
        Vector3 direction = endPos - startPos;

        GameObject newBar = Instantiate(this.Barriere, startPos, Quaternion.Euler(0, 0, 0));
        newBar.GetComponent<BarriereController>().BarriereVelocity = newBar.GetComponent<Rigidbody>().velocity = direction.normalized * this.BarriereSpeed;
        var angularSpeed = Vector3.zero;
        //angularSpeed.x = Random.Range(0.0f, this.BarriereAngularSpeed);
        angularSpeed.y = Random.Range(this.MinBarriereAngularSpeed, this.BarriereAngularSpeed);
        //angularSpeed.z = this.BarriereAngularSpeed - (angularSpeed.x + angularSpeed.y);
        newBar.GetComponent<BarriereController>().AngularSpeed = angularSpeed * this.BarriereSpeed;
    }

    Vector3 CalculatePosition(float angle)
    {
        Vector3 pos = Vector3.zero;
        pos.x = this.Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.z = this.Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        pos.y = this.BarriereHeight;
        return pos;
    }
}
