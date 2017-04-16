using Assets.Scripts.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Control
{
    public class HunterFollowState: HunterState
    {
        private float _followTimer = 0.5f;

        public HunterFollowState(HunterController hunter) : base(hunter, null)
        {
        }

        public override void Enter()
        {
            //this._hunterAI.HunterAnimator.SetTrigger("HunterFear");
            this._hunterController.HunterAnimator.SetTrigger("Follow");
            this._hunterController.HunterAnimator.SetFloat("MoveSpeed", this._hunterController.MoveSpeed);
        }

        public override void OnStateUpdate()
        {
            if ((this._followTimer -= Time.deltaTime) <= 0.0f && this._hunterController.IsWithinAttackRange())
            {
                this._hunterController.ChangeState(HunterCharacterStateMachine.HunterState.Attack);
            }
            else if(!this._hunterController.WillFollow())
            {
                this._hunterController.ChangeState(HunterCharacterStateMachine.HunterState.Idle);
            }
            else
            {
                this._hunterController.ProcessMovement();
            }
        }

        public override void Exit()
        {
            this._hunterController.HunterAnimator.SetFloat("MoveSpeed", 0.0f);
        }

        public override HunterCharacterStateMachine.HunterState GetStateType()
        {
            return HunterCharacterStateMachine.HunterState.Follow;
        }

    }
}

