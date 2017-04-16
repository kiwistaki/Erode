using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Control;
using Assets.Scripts.Utils;
using UnityEngine;
using Assets.Scripts.HexGridGenerator;

namespace Assets.PowerUp
{
    public abstract class AbstractPowerUp : MonoBehaviour
    {
        public enum PowerUpType
        {
            SuperSpeed,
            ScoreMultiplier,
            SlowMotion,
            CircleRepair,
            ThreeWayRepair,
            Ammo,
            PowerUpCount
        }

        public float Lifetime = 7.0f;
        public float Duration = 5.0f;

        private float _lifetime = 0.0f;
        protected PlayerController _playerController = null;

        protected void Awake()
        {
            this._lifetime = this.Lifetime;
        }

        void Update()
        {
            if ((this._lifetime -= Utils.getRealDeltaTime()) <= 0)
            {
                //Player controller is null only if player picked up the bonus
                if (this._playerController != null)
                {
                    this.DeactivatePowerUp();
                }
                this.Expire();
            }
            this.UpdatePowerUp();
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player")
            {
                //Reset the lifetime
                this._lifetime = this.Duration;
                this._playerController = collider.GetComponent<PlayerController>();
				this._playerController.GetComponents<AudioSource> () [3].Play ();

                //Disable collision only on physical powerup objects
                if (this.gameObject.GetComponent<Tile>() == null)
                {
                    this.GetComponent<MeshRenderer>().enabled = false;
                    this.GetComponent<BoxCollider>().enabled = false;
                }

                this.ActivatePowerUp();
            }
        }

        //Activate the powerup effect
        protected abstract void ActivatePowerUp();
        //Update the powerup effect (after picked up)
        protected abstract void UpdatePowerUp();
        //Deactivate the powerup effect
        protected abstract void DeactivatePowerUp();
        //The power-up expires (no pick up)
        protected abstract void Expire();
    }
}
