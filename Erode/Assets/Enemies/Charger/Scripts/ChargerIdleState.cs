using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control{
    public class ChargerIdleState : ChargerState {

        public ChargerIdleState(ChargerController charger)
            : base(charger, null)
            {
            }

            public override void Enter()
            {

            }


            public override void OnStateUpdate()
            {
                if (this._chargerController.WillCharge())
                {
                    this._chargerController.ChangeState(ChargerStateMachine.ChargerState.Shout);
                }
            }

            public override void Exit()
            {
            }

            public override ChargerStateMachine.ChargerState GetStateType()
            {
                return ChargerStateMachine.ChargerState.Idle;
            }
    }
}
