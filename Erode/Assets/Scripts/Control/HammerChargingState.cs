using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HammerChargingState : PlayerState
    {
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
        }

        public override void OnStateUpdate()
        {
            //Capture inputs
            var input = Input.GetAxisRaw("Fire1");
            if (input > 0.0f)
            {
                this._playerController.ProcessRotationInput(0.5f);
            }
            else
            {
                //Release the kraken!
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.HammerChargedStrike);
            }
        }

        public override void Exit()
        {
            this._playerController.HunterAttackEvent -= this._playerController.HitByHunterAttack;
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.HammerCharging;
        }
    }
}
