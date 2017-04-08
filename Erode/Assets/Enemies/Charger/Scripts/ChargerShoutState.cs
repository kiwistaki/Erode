using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ChargerShoutState : ChargerState
    {

        public ChargerShoutState(ChargerController charger)
            : base(charger, null)
        {
        }

        public override void Enter()
        {
            this._chargerController.ShoutAnimCompleteEvent += this._chargerController.ShoutAnimComplete;
            this._chargerController.ChargerAnimator.SetTrigger("DetectPlayer");
        }


        public override void OnStateUpdate()
        {
            this._chargerController.turnTowardsTarget();
        }

        public override void Exit()
        {
            this._chargerController.ShoutAnimCompleteEvent -= this._chargerController.ShoutAnimComplete;
            
        }

        public void ShoutAnimComplete()
        {
            this._chargerController.ChangeState(ChargerStateMachine.ChargerState.Run);
        }

        public override ChargerStateMachine.ChargerState GetStateType()
        {
            return ChargerStateMachine.ChargerState.Shout;
        }
    }
}
