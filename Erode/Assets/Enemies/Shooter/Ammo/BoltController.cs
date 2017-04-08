using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    public float Speed = 3.0f;
    public float PlayerStunnedTime = 0.5f;
    private Vector3 _direction;
    private Transform _player;

	public void Start ()
    {
        //int angle = Random.Range(0, 90);
        this._player = GameObject.FindGameObjectWithTag("Player").transform;
        this._direction = this._player.position - this.transform.position;
        this._direction.y = 0;
        //this._direction.x *= Mathf.Sin(angle);
        //this._direction.y = 0.0f;
        //this._direction.z *= Mathf.Sin(angle);
        this.transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
    }

    public void Update()
    {
        this.transform.Translate(this._direction * this.Speed * Time.deltaTime, Space.World);
    }

    public void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Bolt":
                break;

            case "Player":
                other.GetComponent<PlayerController>().OnShooterAttackEvent(PlayerStunnedTime);
                goto default;

            default:
                Destroy(this.gameObject);
                break;
        }
    }
}
