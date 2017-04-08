using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HunterCharacterStateMachine : AbstractStateMachine
    {

        public enum HunterState
        {
            Undefined = -1,
            Idle,
            Follow,
            Attack,
            Knockbacked,
            Die
        }

        private readonly HunterController _hunterController;

        public HunterCharacterStateMachine(HunterController hunterController)
            : base(new HunterIdleState(hunterController))
        {
            this._hunterController = hunterController;
        }
        
        public void ChangeState(HunterState state, object args)
        {
            switch (state)
            {
                case HunterState.Undefined:
                    throw new UnityException("HunterCharacterStateMachine::ChangeState: HunterState.Undefined");
                case HunterState.Idle:
                    base.ChangeState(new HunterIdleState(this._hunterController));
                    break;
                case HunterState.Follow:
                    base.ChangeState(new HunterFollowState(this._hunterController));
                    break;
                case HunterState.Attack:
                    base.ChangeState(new HunterAttackState(this._hunterController, args));
                    break;
                case HunterState.Knockbacked:
                    base.ChangeState(new HunterKnockbackedState(this._hunterController, args));
                    break;
                case HunterState.Die:
                    base.ChangeState(new HunterDieState(this._hunterController));
                    break;
                default:
                    throw new UnityException("HunterCharacterStateMachine::ChangeState: "+ state.ToString() + " IS NOT IMPLEMENTED");
            }
        }

    }
}



