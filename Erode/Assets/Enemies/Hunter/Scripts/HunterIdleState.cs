using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HunterIdleState : HunterState
    {

        public HunterIdleState(HunterController hunter) : base(hunter, null)
        {
        }

        public override void Enter()
        {
        }


        public override void OnStateUpdate()
        {
            if (this._hunterController.WillFollow())
            {
                this._hunterController.ChangeState(HunterCharacterStateMachine.HunterState.Follow);
            }
        }

        public override void Exit()
        {
        }

        public override HunterCharacterStateMachine.HunterState GetStateType()
        {
            return HunterCharacterStateMachine.HunterState.Idle;
        }
    }
}
