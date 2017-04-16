using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ShooterBackoffState : ShooterState
    {

        public ShooterBackoffState(ShooterController shooter) : base(shooter, null)
        {
        }

        public override void Enter()
        {
        }

        public override void OnStateUpdate()
        {
            if (this._shooterController.ShouldContinueBackoff())
            {
                this._shooterController.ProcessBackoffMovement();
            }
            else
            {
                this._shooterController.ChangeState(ShooterCharacterStateMachine.ShooterState.Idle, null);
                //this._shooterController.ShooterAnimator.SetTrigger("ShooterIdle");
            }
        }

        public override void Exit()
        {

        }

        public override ShooterCharacterStateMachine.ShooterState GetStateType()
        {
            return ShooterCharacterStateMachine.ShooterState.Backoff;
        }

    }
}
