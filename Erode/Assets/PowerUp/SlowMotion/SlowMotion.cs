using Assets.PowerUp;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.Scripts.iTween;
using MirzaBeig.ParticleSystems;
using System.Collections.Generic;
using Assets.Scripts.Utils;

namespace Assets.PowerUps.SlowMotion
{
    public class SlowMotion : AbstractPowerUp
    {
        public GameObject ExpirePrefab;
        
        private PowerUpController _powerUpController;

        private new void Awake()
        {
            base.Awake();
            iTween.MoveBy(this.gameObject, iTween.Hash
                ("amount", this.transform.up * 0.3f
                , "time", 1.0f
                , "looptype", iTween.LoopType.pingPong
                , "easetype", iTween.EaseType.easeInOutBack
                ));
            _powerUpController = Camera.main.GetComponent<PowerUpController>();
        }

        protected override void ActivatePowerUp()
        {
            _powerUpController.ActivatePowerUpEffect(PowerUpType.SlowMotion, this.Duration);
        }

        protected override void DeactivatePowerUp()
        {
            _powerUpController.DeactivatePowerUpEffect(PowerUpType.SlowMotion);
        }

        protected override void Expire()
        {
            var particle = Instantiate(this.ExpirePrefab, this.transform.position, this.transform.rotation).GetComponent<ParticleSystems>();
            particle.setPlaybackSpeed(2.0f);
            Destroy(this.gameObject);
        }

        protected override void UpdatePowerUp()
        {
        }
    }
}
