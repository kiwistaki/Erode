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

        public HunterKnockbackedState(HunterAI hunter, object args) : base(hunter, args)
        {
            this._collidingObject = args as GameObject;

            this._collisionImpulse = this._collidingObject.transform.position - this._hunterAI.transform.position;
            this._collisionImpulse.Normalize();
        }

        public override void Enter()
        {
            //Make sure the animator does not think Hunter is moving
            this._hunterAI.HunterAnimator.SetFloat("MoveSpeed", 0.0f);
            //Trigger the animator
            this._hunterAI.HunterAnimator.SetTrigger("StartKnockback");
            //Setting ip timer
            this._knockbackTime = this._hunterAI.AsteroidKnockbackTime;

            this._hunterAI.AsteroidCollisionEvent -= this._hunterAI.DefaultAsteroidCollision;
        }

        public override void OnStateUpdate()
        {
            if((this._knockbackTime -= Time.deltaTime) <= 0.0f)
            {
                this.KnockbackComplete();
            }
            else
            {
                this._hunterAI.HunterCharacterController.Move(this._hunterAI.AsteroidKnockbackStrenght * Time.deltaTime * -this._collisionImpulse * Mathf.Pow(this._knockbackTime / this._hunterAI.AsteroidKnockbackTime, 3));
                this._hunterAI.HunterCharacterController.Move((this._hunterAI.AsteroidAirKnockbackStrenght * this._hunterAI.Gravity * Mathf.Pow(this._knockbackTime / this._hunterAI.AsteroidKnockbackTime, 3)) * Time.deltaTime * Vector3.up);
            }
        }

        public override void Exit()
        {
            //Trigger the animator
            this._hunterAI.HunterAnimator.SetTrigger("EndKnockback");
            this._hunterAI.AsteroidCollisionEvent += this._hunterAI.DefaultAsteroidCollision;
        }

        public void KnockbackComplete()
        {
            this._hunterAI.ChangeState(HunterCharacterStateMachine.HunterState.Idle);  
        }


        public override HunterCharacterStateMachine.HunterState GetStateType()
        {
            return HunterCharacterStateMachine.HunterState.Knockbacked;
        }
    }
}

