using Assets.PowerUp;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.Scripts.iTween;
using MirzaBeig.ParticleSystems;

namespace Assets.PowerUps.SlowMotion
{
    public class SlowMotion : AbstractPowerUp
    {
        public float DurationOverride = 9f;
        public float LifetimeOverride = 4.5f;
        public GameObject ExpirePrefab;

        private Image _slowMotionImage;

        private void Awake()
        {
            base.Awake();
            base.Duration = this.DurationOverride;
            base.Lifetime = this.LifetimeOverride;
            iTween.MoveBy(this.gameObject, iTween.Hash
                ("amount", this.transform.up * 0.3f
                , "time", 1.0f
                , "looptype", iTween.LoopType.pingPong
                , "easetype", iTween.EaseType.easeInOutBack
                ));
            _slowMotionImage = GameObject.Find("SlowMotionPanel").GetComponent<Image>();
        }

        protected override void ActivatePowerUp()
        {
            Time.timeScale = .3f;
            this._playerController.PlayerAnimator.speed = 1 / Time.timeScale;
            _slowMotionImage.enabled = true;
        }

        protected override void DeactivatePowerUp()
        {
            Time.timeScale = 1f;
            this._playerController.PlayerAnimator.speed = Time.timeScale;
            _slowMotionImage.enabled = false;
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
