using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HunterDieState : HunterState
    {
        public HunterDieState(HunterAI hunter) : base(hunter, null)
        {
        }

        public override void Enter()
        {
            this._hunterAI.AsteroidCollisionEvent -= this._hunterAI.DefaultAsteroidCollision;
            this._hunterAI.HunterAnimator.SetTrigger("HunterDie");
            this._hunterAI.GetComponent<CapsuleCollider>().enabled = false;
            this._hunterAI.GetComponent<CharacterController>().enabled = false;
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

