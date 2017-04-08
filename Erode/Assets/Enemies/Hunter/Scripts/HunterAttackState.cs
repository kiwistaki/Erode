using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HunterAttackState : HunterState
    {
        public HunterAttackState(HunterController hunter, object args) : base(hunter, args)
        {
        }

        public override void Enter()
        {
            this._hunterController.HunterAnimator.SetTrigger("HunterAttack");
            this._hunterController.AttackAnimCompleteEvent += this.AttackAnimComplete;
        }

        public override void OnStateUpdate()
        {
        }

        public override void Exit()
        {
            this._hunterController.AttackAnimCompleteEvent -= this.AttackAnimComplete;
        }

        public void AttackAnimComplete()
        {
            this._hunterController.ChangeState(HunterCharacterStateMachine.HunterState.Idle);
        }

        public override HunterCharacterStateMachine.HunterState GetStateType()
        {
            return HunterCharacterStateMachine.HunterState.Attack;
        }
    }
}

