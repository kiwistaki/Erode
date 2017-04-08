using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control{
public class ChargerKnockbackedState : ChargerState {

    private GameObject _collidingObject;
    private float _knockbackTime;
    private Vector3 _collisionImpulse;

    public ChargerKnockbackedState(ChargerController charger, object args) : base(charger, args)
    {
        this._collidingObject = args as GameObject;

        this._collisionImpulse = this._collidingObject.transform.position - this._chargerController.transform.position;
        this._collisionImpulse.Normalize();
    }

    public override void OnStateUpdate() 
    {
        {
            if ((this._knockbackTime -= Time.deltaTime) <= 0.0f)
            {
                this.KnockbackComplete();
            }
            else
            {
                this._chargerController.ChargerCharacterController.Move(this._chargerController.AsteroidKnockbackStrenght * Time.deltaTime * -this._collisionImpulse 
                    * Mathf.Pow(this._knockbackTime / this._chargerController.AsteroidKnockbackTime, 3));
                this._chargerController.ChargerCharacterController.Move((this._chargerController.AsteroidAirKnockbackStrenght * this._chargerController.Gravity
                    * Mathf.Pow(this._knockbackTime / this._chargerController.AsteroidKnockbackTime, 3)) * Time.deltaTime * Vector3.up);
            }
        }

    }

    public override void Enter()
    {
        //Trigger the animator
        this._chargerController.ChargerAnimator.SetTrigger("StartKnockback");
        this._chargerController.AsteroidCollisionEvent -= this._chargerController.DefaultCollision;

        //Setting ip timer
        this._knockbackTime = this._chargerController.AsteroidKnockbackTime;
    }

    public override void Exit()
    {
        //Trigger the animator
        this._chargerController.ChargerAnimator.SetTrigger("EndKnockback");
        this._chargerController.AsteroidCollisionEvent += this._chargerController.DefaultCollision;
    }

    public void KnockbackComplete()
    {
        this._chargerController.ChangeState(ChargerStateMachine.ChargerState.Idle);
    }


    public override ChargerStateMachine.ChargerState GetStateType()
    {
        return ChargerStateMachine.ChargerState.Knockbacked;
    }
}

}
