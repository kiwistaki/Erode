using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class ChargerDieState : ChargerState
    {
        public ChargerDieState(ChargerController charger)
            : base(charger, null)
        {
        }

        public override void Enter()
        {
            this._chargerController.AsteroidCollisionEvent -= this._chargerController.DefaultCollision;
            this._chargerController.ChargerAnimator.SetTrigger("ChargerDie");
            this._chargerController.GetComponent<CapsuleCollider>().enabled = false;
            this._chargerController.GetComponent<CharacterController>().enabled = false;
        }

        public override void Exit()
        {
        }

        public override void OnStateUpdate()
        {
        }


        public override ChargerStateMachine.ChargerState GetStateType()
        {
            return ChargerStateMachine.ChargerState.Die;
        }
    }
}
