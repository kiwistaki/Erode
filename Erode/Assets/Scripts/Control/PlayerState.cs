using Assets.Scripts.StateMachine;

namespace Assets.Scripts.Control
{
    public abstract class PlayerState : AbstractState
    {
        protected readonly PlayerController _playerController = null;
        protected readonly object _args = null;

        protected PlayerState(PlayerController player, object args)
        {
            this._playerController = player;
            this._args = args;
        }

        public abstract PlayerCharacterStateMachine.PlayerStates GetStateType();
    }
}
