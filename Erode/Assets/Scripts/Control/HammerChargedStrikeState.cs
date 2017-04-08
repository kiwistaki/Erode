namespace Assets.Scripts.Control
{
    public class HammerChargedStrikeState : PlayerState
    {
        public HammerChargedStrikeState(PlayerController player, object args)
            : base(player, args)
        {

        }

        public override void Enter()
        {
            //Trigger the animator
            this._playerController.PlayerAnimator.SetTrigger("HammerStrikeRelease");
            //Subscribe end event
            this._playerController.StrikeAnimCompleteEvent += this.StrikeAnimCompleteEvent;
            //Unsubscribe default knockback
            this._playerController.AsteroidCollisionEvent -= this._playerController.DefaultCollision;
            //Show hammer
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.Hammer);
            this._playerController.SetHammerType((HammerController.HammerType)this._args);
        }

        public override void OnStateUpdate()
        {
        }

        public override void Exit()
        {
            this._playerController.StrikeAnimCompleteEvent -= this.StrikeAnimCompleteEvent;
            this._playerController.AsteroidCollisionEvent += this._playerController.DefaultCollision;

            this._playerController.CleanHammerChargeVfx();
        }

        private void StrikeAnimCompleteEvent()
        {
            this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.IdleRun);
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.HammerStrike;
        }
    }
}
