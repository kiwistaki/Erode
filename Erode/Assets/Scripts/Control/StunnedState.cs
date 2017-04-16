using UnityEngine;

namespace Assets.Scripts.Control
{
    public class StunnedState : PlayerState
    {
        private float _stunTime;
        private float _timeBeforeKnockback = 0.1f;
        private bool _knockbackEnabled = false;

        //(float)args is the amount of time stunned
        public StunnedState(PlayerController player, object args)
            : base(player, args)
        {
            //Setup
            this._stunTime = (float)args;
        }

        public override void Enter()
        {
            //Make sure the animator does not think we're moving
            this._playerController.PlayerAnimator.SetFloat("MoveSpeed", 0.0f);
            //Trigger the animator
            this._playerController.PlayerAnimator.SetTrigger("StartStunned");
            //Hide weapons
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.None);
            //Disable knnockback
            this._playerController.AsteroidCollisionEvent -= this._playerController.DefaultCollision;

            this._playerController.EMPEvent -= this._playerController.InjuredPlayer;
            this._playerController.EMPEvent += this._playerController.InjuredPlayerDelayed;
            this._playerController.GetComponents<AudioSource>()[0].Play();
        }

        public override void OnStateUpdate()
        {
            if ((this._stunTime -= Time.deltaTime) <= 0.0f)
            {
                this.StunComplete();
            }
            if (!this._knockbackEnabled && (this._timeBeforeKnockback -= Time.deltaTime) <= 0.0f)
            {
                //enable knockback
                this._playerController.AsteroidCollisionEvent += this._playerController.DefaultCollision;
                this._knockbackEnabled = true;
            }
        }

        public override void Exit()
        {
            if (this._stunTime <= 0.0f)
            {
                //Stun finished normally
                this._playerController.PlayerAnimator.SetTrigger("EndStunned");
            }
            else
            {
                if (!this._knockbackEnabled)
                {
                    this._playerController.AsteroidCollisionEvent += this._playerController.DefaultCollision;
                }
            }
            this._playerController.EMPEvent -= this._playerController.InjuredPlayerDelayed;
            this._playerController.EMPEvent += this._playerController.InjuredPlayer;
        }

        private void StunComplete()
        {
            this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.IdleRun);
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.Stunned;
        }
    }
}
