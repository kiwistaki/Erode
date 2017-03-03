using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class SpinState : PlayerState
    {
        private float _time = 0.0f;
        private bool _released = false;
        private bool _releasedExit = false;

        public SpinState(PlayerController player) 
            : base(player, null)
        {
        }

        public override void Enter()
        {
            //Trigger the animator
            this._playerController.PlayerAnimator.SetTrigger("SpinStart");
            //Show hammer
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.Hammer);
            this._playerController.SetHammerType(HammerController.HammerType.Spin);
        }

        public override void OnStateUpdate()
        {
            //Checking if player released the Spin button
            if (Input.GetAxisRaw("Fire3") == 0.0f)
            {
                    this._released = true;
            }
            //We need to wait a minimum time before leaving the Spin
            if ((this._time += Time.deltaTime) >= this._playerController.SpinMinTime)
            {
                if (this._released)
                {
                    var overSpin = Mathf.Clamp(this._time, this._playerController.SpinMinTime/2.0f, this._playerController.SpinMoveSpeedDecay/2.0f);
                    if (overSpin > 3*this._playerController.SpinMinTime)
                    {
                        this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.Stunned, overSpin*0.5f);
                    }
                    else
                    {
                        this._releasedExit = true;
                        this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.IdleRun);
                    }
                }
            }
            this.Spin();
        }

        private void Spin()
        {
            var decayMultiplier = Mathf.Max(0.0f, 1-this._time/this._playerController.SpinMoveSpeedDecay);
            this._playerController.ProcessMovementRotationFreeInput(this._playerController.SpinMoveSpeed * decayMultiplier, 0.0f);
            this._playerController.transform.Rotate(this._playerController.transform.up,this._playerController.SpinRotateSpeed*360*Time.deltaTime);
        }

        public override void Exit()
        {
            //Trigger the animator only if we exit correctly
            if (this._releasedExit)
            {
                this._playerController.PlayerAnimator.SetTrigger("SpinEnd");
            }
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.Spin;
        }
    }
}
