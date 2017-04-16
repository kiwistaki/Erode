using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ShooterDieState : ShooterState
    {
        public ShooterDieState(ShooterController shooter) : base(shooter, null)
        {
        }

        public override void Enter()
        {
            this._shooterController.AsteroidCollisionEvent -= this._shooterController.DefaultAsteroidCollision;
            this._shooterController.CometCollisionEvent -= this._shooterController.DefaultAsteroidCollision;
            this._shooterController.AttackAnimCompleteEvent -= this._shooterController.DefaultShootAnimComplete;

            this._shooterController.ShooterAnimator.SetTrigger("ShooterDie");
            this._shooterController.GetComponent<CapsuleCollider>().enabled = false;
        }


        public override void OnStateUpdate()
        {

        }

        public override void Exit()
        {
        }

        public override ShooterCharacterStateMachine.ShooterState GetStateType()
        {
            return ShooterCharacterStateMachine.ShooterState.Die;
        }
    }
}
