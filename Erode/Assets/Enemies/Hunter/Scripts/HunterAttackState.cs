using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HunterAttackState : HunterState
    {
        public HunterAttackState(HunterAI hunter, object args) : base(hunter, args)
        {
        }

        public override void Enter()
        {
            this._hunterAI.HunterAnimator.SetTrigger("HunterAttack");
            this._hunterAI.AttackAnimCompleteEvent += this.AttackAnimComplete;
        }

        public override void OnStateUpdate()
        {
        }

        public override void Exit()
        {
            this._hunterAI.AttackAnimCompleteEvent -= this.AttackAnimComplete;
        }

        public void AttackAnimComplete()
        {
            this._hunterAI.ChangeState(HunterCharacterStateMachine.HunterState.Idle);
        }

        public override HunterCharacterStateMachine.HunterState GetStateType()
        {
            return HunterCharacterStateMachine.HunterState.Attack;
        }
    }
}

