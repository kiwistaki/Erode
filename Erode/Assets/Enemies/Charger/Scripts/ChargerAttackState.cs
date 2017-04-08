using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control 
{
    public class ChargerAttackState : ChargerState
    {

        private GameObject _collidingObject;


        public ChargerAttackState(ChargerController charger, object args)
            : base(charger, null)
        {
        }

        public override void Enter()
        {
            this._chargerController.ChargerAnimator.SetTrigger("AttackPlayer");
            this._chargerController.AttackAnimCompleteEvent += this.AttackAnimComplete;
        }

        public override void OnStateUpdate()
        {

        }

        public override void Exit()
        {
            this._chargerController.AttackAnimCompleteEvent -= this.AttackAnimComplete;
        }

        public void AttackAnimComplete()
        {
            this._chargerController.ChangeState(ChargerStateMachine.ChargerState.Idle);
        }


        public override ChargerStateMachine.ChargerState GetStateType()
        {
            return ChargerStateMachine.ChargerState.Attack;
        }
    }
}

