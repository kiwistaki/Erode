using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{

public class ChargerRunState : ChargerState {

            public ChargerRunState(ChargerController charger)
                : base(charger, null)
                {
                }

                public override void Enter()
                {
                    this._chargerController.chargeDestination();
                }

                public override void OnStateUpdate()
                {
                    
                    this._chargerController.chargePlayer();

                    if (_chargerController.hasReachedOldPlayerPos()) 
                    {
                        this._chargerController.ChangeState(ChargerStateMachine.ChargerState.Attack);
                    }
                    

                }

                public override void Exit()
                {
                    //this._chargerController.ChargerAnimator.SetFloat("MoveSpeed", 0.0f);
                }

                public override ChargerStateMachine.ChargerState GetStateType()
                {
                    return ChargerStateMachine.ChargerState.Run;
                }
        }
}