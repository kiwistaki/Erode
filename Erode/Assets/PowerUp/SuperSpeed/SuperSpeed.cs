using Assets.Scripts.iTween;
using MirzaBeig.ParticleSystems;
using UnityEngine;

namespace Assets.PowerUp.SuperSpeed
{
    public class SuperSpeed : AbstractPowerUp
    {
        public GameObject TrailPrefab;
        public GameObject ExpirePrefab;

        private GameObject _trail;

        void Awake()
        {
            base.Awake();
            iTween.MoveBy(this.gameObject, iTween.Hash
                ( "amount", this.transform.up*0.3f
                , "time", 1.0f
                , "looptype", iTween.LoopType.pingPong
                , "easetype", iTween.EaseType.easeInOutBack
                ));
        }

        protected override void ActivatePowerUp()
        {
            this._playerController.SpeedModifier += 0.5f;
            this._trail = Instantiate(this.TrailPrefab, this._playerController.transform);
            this._trail.transform.localPosition = this.TrailPrefab.transform.localPosition;
        }

        protected override void UpdatePowerUp()
        {
            
        }

        protected override void DeactivatePowerUp()
        {
            this._playerController.SpeedModifier -= 0.5f;
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
