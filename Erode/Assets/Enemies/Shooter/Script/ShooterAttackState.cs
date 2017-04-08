using System;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ShooterAttackState : ShooterState
    {

        public ShooterAttackState(ShooterController shooter, object args) : base(shooter, args)
        {
        }

        public override void Enter()
        {
            this._shooterController.ShooterAnimator.SetTrigger("AttackStart");
            this._shooterController.Shoot();
        }

        public override void OnStateUpdate()
        {
        }

        public override void Exit()
        {
        }

        public override ShooterCharacterStateMachine.ShooterState GetStateType()
        {
            return ShooterCharacterStateMachine.ShooterState.Attack;
        }
    }
}
