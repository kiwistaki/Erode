using Assets.Scripts.iTween;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PowerUp.ScoreBoost 
{


    public class ScoreBoost : AbstractPowerUp
    {
        private PowerUpController _powerUpController;
        public float RotationSpeed = 60f;

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
            _powerUpController.ActivatePowerUpEffect(PowerUpType.ScoreMultiplier, this.Duration);
        }

        protected override void DeactivatePowerUp() 
        {
            _powerUpController.DeactivatePowerUpEffect(PowerUpType.ScoreMultiplier);
        }

        protected override void UpdatePowerUp()
        {
            Vector3 AngularSpeed = new Vector3(0, RotationSpeed, 0);
            this.gameObject.transform.Rotate(AngularSpeed * Time.deltaTime, Space.World);
        }

        protected override void Expire()
        {
            Destroy(this.gameObject);
        }
    }
}

