using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HunterKnockbackedState : HunterState
    {
        private GameObject _collidingObject;
        private float _knockbackTime;
        private Vector3 _collisionImpulse;

        public HunterKnockbackedState(HunterController hunter, object args) : base(hunter, args)
        {
            this._collidingObject = args as GameObject;

            this._collisionImpulse = this._collidingObject.transform.position - this._hunterController.transform.position;
            this._collisionImpulse.Normalize();
        }

        public override void Enter()
        {
            //Make sure the animator does not think Hunter is moving
            this._hunterController.HunterAnimator.SetFloat("MoveSpeed", 0.0f);
            //Trigger the animator
            this._hunterController.HunterAnimator.SetTrigger("StartKnockback");
            //Setting ip timer
            this._knockbackTime = this._hunterController.AsteroidKnockbackTime;

            this._hunterController.AsteroidCollisionEvent -= this._hunterController.DefaultAsteroidCollision;
        }

        public override void OnStateUpdate()
        {
            if((this._knockbackTime -= Time.deltaTime) <= 0.0f)
            {
                this.KnockbackComplete();
            }
            else
            {
                this._hunterController.HunterCharacterController.Move(this._hunterController.AsteroidKnockbackStrenght * Time.deltaTime * -this._collisionImpulse * Mathf.Pow(this._knockbackTime / this._hunterController.AsteroidKnockbackTime, 3));
                this._hunterController.HunterCharacterController.Move((this._hunterController.AsteroidAirKnockbackStrenght * this._hunterController.Gravity * Mathf.Pow(this._knockbackTime / this._hunterController.AsteroidKnockbackTime, 3)) * Time.deltaTime * Vector3.up);
            }
        }

        public override void Exit()
        {
            //Trigger the animator
            this._hunterController.HunterAnimator.SetTrigger("EndKnockback");
            this._hunterController.AsteroidCollisionEvent += this._hunterController.DefaultAsteroidCollision;
        }

        public void KnockbackComplete()
        {
            this._hunterController.ChangeState(HunterCharacterStateMachine.HunterState.Idle);  
        }


        public override HunterCharacterStateMachine.HunterState GetStateType()
        {
            return HunterCharacterStateMachine.HunterState.Knockbacked;
        }
    }
}

