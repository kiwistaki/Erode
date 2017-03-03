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

        private readonly HunterAI _hunterAI;

        public HunterCharacterStateMachine(HunterAI hunterAI)
            : base(new HunterIdleState(hunterAI))
        {
            this._hunterAI = hunterAI;
        }
        
        public void ChangeState(HunterState state, object args)
        {
            switch (state)
            {
                case HunterState.Undefined:
                    throw new UnityException("HunterCharacterStateMachine::ChangeState: HunterState.Undefined");
                    break;
                case HunterState.Idle:
                    base.ChangeState(new HunterIdleState(this._hunterAI));
                    break;
                case HunterState.Follow:
                    base.ChangeState(new HunterFollowState(this._hunterAI));
                    break;
                case HunterState.Attack:
                    base.ChangeState(new HunterAttackState(this._hunterAI, args));
                    break;
                case HunterState.Knockbacked:
                    base.ChangeState(new HunterKnockbackedState(this._hunterAI, args));
                    break;
                case HunterState.Die:
                    base.ChangeState(new HunterDieState(this._hunterAI));
                    break;
                default:
                    throw new UnityException("HunterCharacterStateMachine::ChangeState: "+ state.ToString() + " IS NOT IMPLEMENTED");
                    break;
            }
        }

    }
}



