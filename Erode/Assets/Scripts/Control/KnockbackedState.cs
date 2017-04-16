using UnityEngine;

namespace Assets.Scripts.Control
{
    public class KnockbackedState : PlayerState
    {
        private GameObject _collidingObject;
        private float _knockbackTime;
        private Vector3 _collisionImpulse;

        //(GameObject)args is the colliding object
        public KnockbackedState(PlayerController player, object args)
            : base(player, args)
        {
            //Setup
            this._collidingObject = args as GameObject;
            this._knockbackTime = this._playerController.KnockbackTime;

            this._collisionImpulse = this._collidingObject.transform.position - this._playerController.transform.position;
            //Need to compute the real forward and right
            this._collisionImpulse.y = 0;
            var right = Vector3.Cross(Vector3.up, this._collidingObject.GetComponent<Rigidbody>().velocity.normalized);
            var proj = Vector3.Project(this._collisionImpulse, right);
            var x2 = right.x * proj.x;
            var y2 = right.y * proj.y;
            var z2 = right.z * proj.z;
            bool isRight = x2 >= 0.0f && y2 >= 0.0f && z2 >= 0.0f;
            this._collisionImpulse += (isRight ? right : -right);
            this._collisionImpulse.Normalize();
        }

        public override void Enter()
        {
            //Make sure the animator does not think we're moving
            this._playerController.PlayerAnimator.SetFloat("MoveSpeed", 0.0f);
            //Trigger the animator
            this._playerController.PlayerAnimator.SetTrigger("StartKnockback");
            //Hide weapons
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.None);
            //Disable further knockback
            this._playerController.AsteroidCollisionEvent -= this._playerController.DefaultCollision;
            this._playerController.CometCollisionEvent -= this._playerController.DefaultCollision;
            this._playerController.EMPEvent -= this._playerController.InjuredPlayer;
            this._playerController.EMPEvent += this._playerController.InjuredPlayerDelayed;
            this._playerController.ShooterAttackEvent -= this._playerController.HitByShooterAttack;
            this._playerController.GetComponents<AudioSource>()[1].Play();
        }

        public override void OnStateUpdate()
        {
            if ((this._knockbackTime -= Utils.Utils.getRealDeltaTime()) <= 0.0f)
            {
                this.KnockbackComplete();
            }
            else
            {
                this._playerController.PlayerCharacterController.Move(this._playerController.AsteroidKnockbackStrenght * Utils.Utils.getRealDeltaTime() * - this._collisionImpulse*Mathf.Pow(this._knockbackTime/this._playerController.KnockbackTime,3));
                this._playerController.PlayerCharacterController.Move((this._playerController.AsteroidAirKnockbackStrenght*this._playerController.Gravity*Mathf.Pow(this._knockbackTime/this._playerController.KnockbackTime,3)) * Utils.Utils.getRealDeltaTime() * Vector3.up);
            }
        }

        public override void Exit()
        {
            //Trigger the animator
            Debug.Assert(this._knockbackTime <= 0.0f);
            this._playerController.PlayerAnimator.SetTrigger("EndKnockback");
            //Re-enable knockback
            this._playerController.AsteroidCollisionEvent += this._playerController.DefaultCollision;
            this._playerController.CometCollisionEvent += this._playerController.DefaultCollision;
            this._playerController.EMPEvent -= this._playerController.InjuredPlayerDelayed;
            this._playerController.EMPEvent += this._playerController.InjuredPlayer;
            this._playerController.ShooterAttackEvent += this._playerController.HitByShooterAttack;
        }

        private void KnockbackComplete()
        {
            this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.IdleRun);
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.Knockbacked;
        }
    }
}
