using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class PlayerCharacterStateMachine : AbstractStateMachine
    {
        public enum PlayerStates
        {
            Undefined = -1,
            IdleRun,
            HammerCharging,
            HammerStrike,
            HammerChargedStrike,
            Blitz,
            Spin,
            Knockbacked,
            Stunned,
            Repairing,
            Injured
        }

        private readonly PlayerController _playerController;

        public PlayerCharacterStateMachine(PlayerController playerController)
            : base(new IdleRunState(playerController))
        {
            this._playerController = playerController;
        }

        public void ChangeState(PlayerStates state, object args)
        {
            switch (state)
            {
                case PlayerStates.Undefined:
                    throw new UnityException("PlayerCharacterStateMachine::ChangeState: Usage of undefined enum");

                case PlayerStates.IdleRun:
                    base.ChangeState(new IdleRunState(this._playerController));
                    break;

                case PlayerStates.HammerCharging:
                    base.ChangeState(new HammerChargingState(this._playerController));
                    break;

                case PlayerStates.HammerStrike:
                    base.ChangeState(new HammerStrikeState(this._playerController));
                    break;

                case PlayerStates.HammerChargedStrike:
                    base.ChangeState(new HammerChargedStrikeState(this._playerController, args));;
                    break;

                case PlayerStates.Blitz:
                    base.ChangeState(new BlitzState(this._playerController)); ;
                    break;

                case PlayerStates.Spin:
                    base.ChangeState(new SpinState(this._playerController));
                    break;

                case PlayerStates.Knockbacked:
                    base.ChangeState(new KnockbackedState(this._playerController, args));
                    break;

                case PlayerStates.Stunned:
                    base.ChangeState(new StunnedState(this._playerController, args));
                    break;

                case PlayerStates.Repairing:
                    base.ChangeState(new RepairingState(this._playerController));
                    break;

                case PlayerStates.Injured:
                    base.ChangeState(new InjuredState(this._playerController, args));
                    break;
                default:
                    throw new UnityException("PlayerCharacterStateMachine::ChangeState: " + state.ToString() + " IS NOT IMPLEMENTED");
            }
        }

    }
}
