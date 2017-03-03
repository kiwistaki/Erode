namespace Assets.Scripts.Control
{
    public class HammerChargedStrikeState : PlayerState
    {
        public HammerChargedStrikeState(PlayerController player)
            : base(player, null)
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
            this._playerController.SetHammerType(HammerController.HammerType.Charged);
        }

        public override void OnStateUpdate()
        {
        }

        public override void Exit()
        {
            this._playerController.StrikeAnimCompleteEvent -= this.StrikeAnimCompleteEvent;
            this._playerController.AsteroidCollisionEvent += this._playerController.DefaultCollision;
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
