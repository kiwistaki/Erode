using UnityEngine;

namespace Assets.Scripts.Control
{
    class InjuredState : PlayerState
    {
        private float _injuredTime = 0.0f;
        private float _timer = 0.0f;

        public InjuredState(PlayerController player, object args) : base(player, args)
        {
            this._injuredTime = (float)args;
        }

        public override void Enter()
        {
            this._playerController.PlayerAnimator.SetBool("Injured", true);

            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.None);

            this._playerController.HunterAttackEvent += this._playerController.HitByHunterAttack;
            this._playerController.ShooterAttackEvent += this._playerController.HitByShooterAttack;
        }

        public override void Exit()
        {
            this._playerController.PlayerAnimator.SetBool("Injured", false);

            this._playerController.HunterAttackEvent -= this._playerController.HitByHunterAttack;
            this._playerController.ShooterAttackEvent -= this._playerController.HitByShooterAttack;
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.Injured;
        }

        public override void OnStateUpdate()
        {
            this._playerController.ProcessMovementRotationFreeInput(this._playerController.EMPInjuredSpeed, 1.0f);
            _timer += Time.deltaTime;
            if (_timer >= _injuredTime)
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.IdleRun);
        }
    }
}
