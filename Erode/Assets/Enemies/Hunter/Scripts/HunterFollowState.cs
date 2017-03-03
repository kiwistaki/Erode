using Assets.Scripts.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Control
{
    public class HunterFollowState: HunterState
    {
        public HunterFollowState(HunterAI hunter) : base(hunter, null)
        {
        }


        public override void Enter()
        {
            //this._hunterAI.HunterAnimator.SetTrigger("HunterFear");
            this._hunterAI.HunterAnimator.SetTrigger("Follow");
            this._hunterAI.HunterAnimator.SetFloat("MoveSpeed", this._hunterAI.MoveSpeed);
        }

        public override void OnStateUpdate()
        {
            if (this._hunterAI.IsWithinAttackRange())
            {
                this._hunterAI.ChangeState(HunterCharacterStateMachine.HunterState.Attack);
            }

            if(!this._hunterAI.WillFollow())
            {
                this._hunterAI.ChangeState(HunterCharacterStateMachine.HunterState.Idle);
            }

            this._hunterAI.ProcessMovement();
        }

        public override void Exit()
        {
            this._hunterAI.HunterAnimator.SetFloat("MoveSpeed", 0.0f);
        }

        public override HunterCharacterStateMachine.HunterState GetStateType()
        {
            return HunterCharacterStateMachine.HunterState.Follow;
        }

    }
}

