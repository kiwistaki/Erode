using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ChargerStateMachine : AbstractStateMachine
    {

        public enum ChargerState
        {
            Undefined = -1,
            Idle,
            Knockbacked,
            Shout,
            Run,
            Attack,
            Die
        }

        private readonly ChargerController _chargerController;
        private ChargerState _chargerState = ChargerState.Undefined;

        public ChargerStateMachine(ChargerController chargerController)
            : base(new ChargerIdleState(chargerController))
        {
            this._chargerController = chargerController;
        }

        public int returnEnumValue() 
        {
            return (int)_chargerState;
        }

        public void ChangeState(ChargerState state, object args)
        {
            switch (state)
            {
                case ChargerState.Undefined:
                    throw new UnityException("HunterCharacterStateMachine::ChangeState: HunterState.Undefined");
                case ChargerState.Idle:
                    _chargerState = ChargerState.Idle;
                    base.ChangeState(new ChargerIdleState(this._chargerController));
                    break;
                case ChargerState.Knockbacked:
                    _chargerState = ChargerState.Knockbacked;
                    base.ChangeState(new ChargerKnockbackedState(this._chargerController, args));
                    break;
                case ChargerState.Shout:
                    _chargerState = ChargerState.Shout;
                    base.ChangeState(new ChargerShoutState(this._chargerController));
                    break;
                case ChargerState.Run:
                    _chargerState = ChargerState.Run;
                    base.ChangeState(new ChargerRunState(this._chargerController));
                    break;
                case ChargerState.Attack:
                    _chargerState = ChargerState.Attack;
                    base.ChangeState(new ChargerAttackState(this._chargerController, args));
                    break;
                case ChargerState.Die:
                    _chargerState = ChargerState.Die;
                    base.ChangeState(new ChargerDieState(this._chargerController));
                    break;
                default:
                    throw new UnityException("HunterCharacterStateMachine::ChangeState: " + state.ToString() + " IS NOT IMPLEMENTED");
            }
        }

    }
}
