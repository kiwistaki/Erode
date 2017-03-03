namespace Assets.Scripts.Control
{
    public class HammerStrikeState : PlayerState
    {
        public HammerStrikeState(PlayerController player)
            : base(player, null)
        {

        }

        public override void Enter()
        {
            //Trigger the animator
            this._playerController.PlayerAnimator.SetTrigger("HammerStrikeQuick");
            //Subscribe end event
            this._playerController.StrikeAnimCompleteEvent += this.StrikeAnimCompleteEvent;
            //Subscribe to transition
            this._playerController.AnimTransitionEvent += this.ShowHammer;
        }

        private void ShowHammer()
        {
            //Show hammer
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.Hammer);
            this._playerController.SetHammerType(HammerController.HammerType.Quick);
            //Resubscribe to transition
            this._playerController.AnimTransitionEvent -= this.ShowHammer;
            this._playerController.AnimTransitionEvent += this.HideHammer;
        }

        private void HideHammer()
        {
            //Hide hammer
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.None);
            //Unsubscribe from transition
            this._playerController.AnimTransitionEvent -= this.HideHammer;
        }

        public override void OnStateUpdate()
        {
            this._playerController.ProcessMovementRotationLockInput(1.0f,0.25f);
        }

        public override void Exit()
        {
            //Unsubscribe end event
            this._playerController.StrikeAnimCompleteEvent -= this.StrikeAnimCompleteEvent;
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
