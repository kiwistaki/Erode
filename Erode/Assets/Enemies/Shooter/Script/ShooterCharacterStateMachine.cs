using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ShooterCharacterStateMachine : AbstractStateMachine
    {
        public enum ShooterState
        {
            Undefined = -1,
            Idle,
            Follow,
            Backoff,
            Attack,
            Knockbacked,
            Die
        }

        private readonly ShooterController _shooterController;

        public ShooterCharacterStateMachine(ShooterController shooterController)
            : base(new ShooterIdleState(shooterController))
        {
            this._shooterController = shooterController;
        }

        public void ChangeState(ShooterState state, object args)
        {
            switch (state)
            {
                case ShooterState.Undefined:
                    throw new UnityException("HunterCharacterStateMachine::ChangeState: HunterState.Undefined");
                case ShooterState.Idle:
                    base.ChangeState(new ShooterIdleState(this._shooterController));
                    break;
                case ShooterState.Follow:
                    base.ChangeState(new ShooterFollowState(this._shooterController));
                    break;
                case ShooterState.Backoff:
                    base.ChangeState(new ShooterBackoffState(this._shooterController));
                    break;
                case ShooterState.Attack:
                    base.ChangeState(new ShooterAttackState(this._shooterController, args));
                    break;
                case ShooterState.Knockbacked:
                    base.ChangeState(new ShooterKnockbackedState(this._shooterController, args));
                    break;
                case ShooterState.Die:
                    base.ChangeState(new ShooterDieState(this._shooterController));
                    break;
                default:
                    throw new UnityException("HunterCharacterStateMachine::ChangeState: " + state.ToString() + " IS NOT IMPLEMENTED");
            }
        }
    }
}

