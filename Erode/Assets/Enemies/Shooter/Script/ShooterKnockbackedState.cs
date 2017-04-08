using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ShooterKnockbackedState : ShooterState
    {
        private GameObject _collidingObject;
        private float _knockbackTime;
        private Vector3 _collisionImpulse;

        public ShooterKnockbackedState(ShooterController shooter, object args) : base(shooter, args)
        {
            this._collidingObject = args as GameObject;

            this._collisionImpulse = this._collidingObject.transform.position - this._shooterController.transform.position;
            this._collisionImpulse.Normalize();
        }

        public override void Enter()
        {
            //Trigger the animator
            //this._shooterController.ShooterAnimator.SetTrigger("StartKnockback");
            //Setting ip timer
            this._knockbackTime = this._shooterController.CometKnockbackTime;

            this._shooterController.CometCollisionEvent -= this._shooterController.DefaultAsteroidCollision;
        }


        public override void OnStateUpdate()
        {
            if ((this._knockbackTime -= Time.deltaTime) <= 0.0f)
            {
                this.KnockbackComplete();
            }
            else
            {
                this._shooterController.ShooterCharacterController.Move(this._shooterController.CometKnockbackStrenght * Time.deltaTime * -this._collisionImpulse * Mathf.Pow(this._knockbackTime / this._shooterController.CometKnockbackTime, 3));
                this._shooterController.ShooterCharacterController.Move((this._shooterController.CometAirKnockbackStrenght * this._shooterController.Gravity * Mathf.Pow(this._knockbackTime / this._shooterController.CometKnockbackTime, 3)) * Time.deltaTime * Vector3.up);
            }
        }

        public override void Exit()
        {
            //Reactivate Collision
            this._shooterController.CometCollisionEvent += this._shooterController.DefaultAsteroidCollision;
        }

        public void KnockbackComplete()
        {
            this._shooterController.ChangeState(ShooterCharacterStateMachine.ShooterState.Idle);
        }

        public override ShooterCharacterStateMachine.ShooterState GetStateType()
        {
            return ShooterCharacterStateMachine.ShooterState.Knockbacked;
        }
    }
}
