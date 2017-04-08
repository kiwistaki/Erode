using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HammerChargingState : PlayerState
    {
        private float _chargeTimeIncrement = 0.0f;
        private int _chargeLevel = 0;
        private List<GameObject> _hammerVfx = new List<GameObject>();

        public HammerChargingState(PlayerController player)
            : base(player, null)
        {
        }

        public override void Enter()
        {
            //Make sure the animator does not think we're moving
            this._playerController.PlayerAnimator.SetFloat("MoveSpeed", 0.0f);
            //Trigger the animator
            this._playerController.PlayerAnimator.SetTrigger("HammerStrikeHold");
            //Show hammer
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.Hammer);
            this._playerController.SetHammerType(HammerController.HammerType.Disabled);

            this._playerController.HunterAttackEvent += this._playerController.HitByHunterAttack;
            this._playerController.ShooterAttackEvent += this._playerController.HitByShooterAttack;

            this._chargeTimeIncrement = this._playerController.ChargeTimeIncrement;
            this._playerController.PlayChargingVfx(this._chargeLevel);
        }

        public override void OnStateUpdate()
        {
            //Capture inputs
            var input = Input.GetAxisRaw("Fire1");
            if (input > 0.0f)
            {
                this._playerController.ProcessRotationInput(0.5f);
                if ((this._chargeTimeIncrement -= Utils.Utils.getRealDeltaTime()) <= 0.0f && this._chargeLevel != 2)
                {
                    this._chargeTimeIncrement += this._playerController.ChargeTimeIncrement;
                    this._chargeLevel += 1;
                    this._playerController.PlayChargingVfx(this._chargeLevel);
                    this._playerController.PlayChargingBurstVfx();
                }
            }
            else
            {
                //Release the kraken!
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.HammerChargedStrike, HammerController.HammerType.ChargedLow + this._chargeLevel);
            }
        }

        public override void Exit()
        {
            this._playerController.HunterAttackEvent -= this._playerController.HitByHunterAttack;
            this._playerController.ShooterAttackEvent -= this._playerController.HitByShooterAttack;
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.HammerCharging;
        }

        private void IncreaseChargeLevel()
        {
            
        }
    }
}
