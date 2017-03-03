using Assets.Obstacles.Asteroide;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class BlitzState: PlayerState
    {
        private float _time = 0.0f;
        private bool _releasedButton = false;

        public BlitzState(PlayerController player) 
            : base(player, null)
        {
        }

        public override void Enter()
        {
            //Trigger the animator
            this._playerController.PlayerAnimator.SetTrigger("BlitzStart");
            //Change default knockback
            this._playerController.AsteroidCollisionEvent -= this._playerController.DefaultCollision;
            this._playerController.AsteroidCollisionEvent += this.AsteroidCollision;
            this._playerController.CometCollisionEvent -= this._playerController.DefaultCollision;
            this._playerController.CometCollisionEvent += this.CometCollision;
            //Register to hunter collision
            this._playerController.HunterCollisionEvent += this.HunterCollision;
            //Show hammer
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.Hammer);
            this._playerController.SetHammerType(HammerController.HammerType.Disabled);
        }

        public override void OnStateUpdate()
        {
            //Checking if player released the Blitz button
            if (Input.GetAxisRaw("Fire2") == 0.0f)
            {
                this._releasedButton = true;
            }
            //Making sure we're not in Blitz too long
            if ((this._time += Time.deltaTime) >= this._playerController.BlitzMaxTime)
            {
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.HammerChargedStrike);
            }
            else if (this._time >= this._playerController.BlitzMinTime)
            {
                if (this._releasedButton)
                {
                    this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.HammerChargedStrike);
                }
            }
            this.BlitzMovement();
        }

        public override void Exit()
        {
            //Trigger the animator
            this._playerController.PlayerAnimator.SetTrigger("BlitzEnd");
            //Return to default knockback
            this._playerController.AsteroidCollisionEvent -= this.AsteroidCollision;
            this._playerController.AsteroidCollisionEvent += this._playerController.DefaultCollision;
            this._playerController.CometCollisionEvent -= this.CometCollision;
            this._playerController.CometCollisionEvent += this._playerController.DefaultCollision;
            //Unregister hunter collisions
            this._playerController.HunterCollisionEvent -= this.HunterCollision;
        }

        public void AsteroidCollision(GameObject asteroid)
        {
            this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.Stunned, this._playerController.BlitzStunTimeAsteroid);
            asteroid.GetComponent<AsteroideController>().CollisionWithBlitz(this._playerController.gameObject);
            //Vfx
            this._playerController.PlayHitEffect(this._playerController.transform.position + (asteroid.transform.position - this._playerController.transform.position) * 0.5f);
        }

        public void CometCollision(GameObject comet)
        {
            this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.Knockbacked, comet);
            //Vfx
            this._playerController.PlayHitEffect(this._playerController.transform.position + (comet.transform.position - this._playerController.transform.position) * 0.5f);
        }

        public void HunterCollision(GameObject hunter)
        {
            //HunterAi.OnAsteroidCollision is actually a generic collision funtion
            hunter.GetComponent<HunterAI>().OnAsteroidCollision(this._playerController.gameObject);
            this._playerController.PlayHitEffect(this._playerController.transform.position + (hunter.transform.position - this._playerController.transform.position) * 0.5f);
        }

        private void BlitzMovement()
        {
            //Capture inputs
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            //Diagonal movement speed needs to be clamped.
            if (input.magnitude > 1.0f)
            {
                input.Normalize();
            }

            //Apply rotation
            if (input != Vector3.zero)
            {
                this._playerController.transform.rotation =
                    Quaternion.Slerp(this._playerController.transform.rotation, Quaternion.LookRotation(input.normalized), this._playerController.BlitzTurnSpeed * Time.deltaTime);
            }
            
            //Apply movement
            this._playerController.MovePlayer(this._playerController.transform.forward, this._playerController.BlitzSpeed);
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.Blitz;
        }
    }
}
