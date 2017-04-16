using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    public float Speed = 3.0f;
    public float PlayerStunnedTime = 0.5f;
    private bool _hasBeenReversed = false;
    private Vector3 _direction;
    private Transform _player;

	public void Start ()
    {
        //int angle = Random.Range(0, 90);
        this._player = GameObject.FindGameObjectWithTag("Player").transform;
	    var offset = new Vector3(Random.value * 3.0f - 1.5f, 0.0f, Random.value * 3.0f - 1.5f);
        this._direction = this._player.position + offset - this.transform.position;
        this._direction.y = 0;
        this._direction.Normalize();
        //this._direction.x *= Mathf.Sin(angle);
        //this._direction.y = 0.0f;
        //this._direction.z *= Mathf.Sin(angle);
        this.transform.rotation = Quaternion.LookRotation(this._direction, Vector3.up);
    }

    public void Update()
    {
        this.transform.Translate(this._direction * this.Speed * Time.deltaTime, Space.World);
        if(this.transform.position.magnitude >= 50)
            Destroy(this.gameObject);
    }

    public void ReverseDirection()
    {
        if (!this._hasBeenReversed)
        {
            this._direction *= -1;
            this._hasBeenReversed = true; 
        }   
    }

    public void TimeToDie()
    {
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Bolt":
                break;

            case "Player":
                other.GetComponent<PlayerController>().OnShooterAttackEvent(this.gameObject, PlayerStunnedTime);
                break;

            case "Hammer":
                other.GetComponent<HammerController>().OnBoltCollision(this.gameObject, PlayerStunnedTime);

                break;

            default:
                TimeToDie();
                break;
        }
    }
}
