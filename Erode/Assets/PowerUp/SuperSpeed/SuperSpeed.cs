using System;
using Assets.Scripts.iTween;
using MirzaBeig.ParticleSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Utils;

namespace Assets.PowerUp.SuperSpeed
{
    public class SuperSpeed : AbstractPowerUp
    {
        public GameObject TrailPrefab;
        public GameObject ExpirePrefab;

        private GameObject _trail;
        private bool _timersLock = false;
        private PowerUpController _powerUpController;

        new void Awake()
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
            this._trail = Instantiate(this.TrailPrefab, this._playerController.transform);
            this._trail.transform.localPosition = this.TrailPrefab.transform.localPosition;
            _powerUpController.ActivatePowerUpEffect(PowerUpType.SuperSpeed, this.Duration);
        }

        protected override void UpdatePowerUp()
        {
        }

        protected override void DeactivatePowerUp()
        {
            _powerUpController.DeactivatePowerUpEffect(PowerUpType.SuperSpeed);
            Destroy(this._trail);
        }

        protected override void Expire()
        {
            var particle = Instantiate(this.ExpirePrefab, this.transform.position, this.transform.rotation).GetComponent<ParticleSystems>();
            particle.setPlaybackSpeed(2.0f);
            Destroy(this.gameObject);
        }
    }
}
