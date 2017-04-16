using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ShooterFollowState : ShooterState
    {
        public ShooterFollowState(ShooterController shooter) : base(shooter, null)
        {
        }

        public override void Enter()
        {
        }

        public override void OnStateUpdate()
        {
            if (this._shooterController.ShouldContinueFollowing())
            {
                this._shooterController.ChangeState(ShooterCharacterStateMachine.ShooterState.Idle);
            }
            this._shooterController.ProcessMovement();
        }

        public override void Exit()
        {
        }

        public override ShooterCharacterStateMachine.ShooterState GetStateType()
        {
            return ShooterCharacterStateMachine.ShooterState.Follow;
        }
    }
}
