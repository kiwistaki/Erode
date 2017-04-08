using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HunterDieState : HunterState
    {
        public HunterDieState(HunterController hunter) : base(hunter, null)
        {
        }

        public override void Enter()
        {
            this._hunterController.AsteroidCollisionEvent -= this._hunterController.DefaultAsteroidCollision;
            this._hunterController.HunterAnimator.SetTrigger("HunterDie");
            this._hunterController.GetComponent<CapsuleCollider>().enabled = false;
            this._hunterController.GetComponent<CharacterController>().enabled = false;
        }

        public override void Exit()
        {
        }

        public override void OnStateUpdate()
        {

        }


        public override HunterCharacterStateMachine.HunterState GetStateType()
        {
            return HunterCharacterStateMachine.HunterState.Die;
        }
    }
}

