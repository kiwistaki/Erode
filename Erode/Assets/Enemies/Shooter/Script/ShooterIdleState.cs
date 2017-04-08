using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ShooterIdleState : ShooterState
    {
        private float _shootTimer = 0.0f;

        public ShooterIdleState(ShooterController shooter) : base(shooter, null)
        {
        }

        public override void Enter()
        {
            this._shootTimer = this._shooterController.TimeBetweenBolt;
        }


        public override void OnStateUpdate()
        {
            this._shootTimer -= Time.deltaTime;
            if (this._shooterController.WillFollow())
            {
                this._shooterController.ChangeState(ShooterCharacterStateMachine.ShooterState.Follow);
            }
            else if (this._shooterController.WillBackoff())
            {
                this._shooterController.ChangeState(ShooterCharacterStateMachine.ShooterState.Backoff);
            }
            else if (this._shootTimer <= 0)
            {
                this._shooterController.ChangeState(ShooterCharacterStateMachine.ShooterState.Attack);
            }
            else
            {
                this._shooterController.ProcessOrbitMovement();
                this._shooterController.transform.rotation = Quaternion.Slerp(this._shooterController.transform.rotation,
                Quaternion.LookRotation(this._shooterController.Target.transform.position - this._shooterController.transform.position), Time.deltaTime * 5);
            }
        }

        public override void Exit()
        {
        }

        public override ShooterCharacterStateMachine.ShooterState GetStateType()
        {
            return ShooterCharacterStateMachine.ShooterState.Idle;
        }
    }
}

