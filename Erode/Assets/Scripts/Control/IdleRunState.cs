using UnityEngine;

namespace Assets.Scripts.Control
{
    public class IdleRunState : PlayerState
    {
        private float _strikePressTimer = 0.0f;
        private float _blitzPressTimer = 0.0f;

        public IdleRunState(PlayerController player)
            : base(player, null)
        {
        }

        public override void Enter()
        {
            //Hide weapons
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.None);
            //Register to hunter attack event
            this._playerController.HunterAttackEvent += this._playerController.HitByHunterAttack;
        }

        public override void OnStateUpdate()
        {
            this._playerController.ProcessMovementRotationFreeInput(1.0f, 1.0f);

            this.CheckHammerStrike();
            this.CheckBlitz();
            this.CheckSpin();
            this.CheckRepair();
        }

        private void CheckBlitz()
        {
            var input = Input.GetAxisRaw("Fire2");
            if (input > 0.0f)
            {
                if ((this._blitzPressTimer += Time.deltaTime) >= 0.05f)
                {
                    this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.Blitz);
                }
            }
            else
            {
                this._blitzPressTimer = 0.0f;
            }
        }

        private void CheckSpin()
        {
            var input = Input.GetAxisRaw("Fire3");
            if (input > 0.0f)
            {
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.Spin);
            }
        }

        private void CheckHammerStrike()
        {
            //Charged strike or standard strike
            var input = Input.GetAxisRaw("Fire1");
            if (input > 0.0f)
            {
                //Holding the hammer strike button
                this._strikePressTimer += Time.deltaTime;
                if (this._strikePressTimer >= this._playerController.HammerChargeMinimalHold)
                {
                    //Hammer strike button held
                    this._strikePressTimer = 0.0f;
                    this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.HammerCharging);
                }
            }
            else if (this._strikePressTimer > 0.0f && this._strikePressTimer < this._playerController.HammerChargeMinimalHold)
            {
                //Released the hammer strike button quickly
                this._strikePressTimer = 0.0f;
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.HammerStrike);
            }
        }

        private void CheckRepair()
        {
            var input = Input.GetAxisRaw("Fire4");
            if(input > 0.0f)
            {
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.Repairing);
            }
        }

        public override void Exit()
        {
            this._playerController.HunterAttackEvent -= this._playerController.HitByHunterAttack;
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.IdleRun;
        }
    }
}
