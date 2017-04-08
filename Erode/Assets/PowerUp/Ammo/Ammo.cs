using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.iTween;
using UnityEngine;

namespace Assets.PowerUp.Ammo
{
    public class Ammo : AbstractPowerUp
    {
        private Quaternion _rotation;

        new void Awake()
        {
            base.Awake();
            _rotation = this.transform.rotation;
            this.transform.rotation = Quaternion.Euler(this._rotation.x, this._rotation.y, 90.0f);
            iTween.MoveBy(this.gameObject, iTween.Hash
               ("amount", this.transform.up * 0.3f
               , "time", 1.0f
               , "looptype", iTween.LoopType.pingPong
               , "easetype", iTween.EaseType.easeInOutBack
               ));

        }

        protected override void ActivatePowerUp()
        {
            this._playerController.AmmoCount = Mathf.Clamp(this._playerController.AmmoCount + this._playerController.MaxAmmo/2, 0, this._playerController.MaxAmmo);
        }

        protected override void UpdatePowerUp()
        {
        }

        protected override void DeactivatePowerUp()
        {
        }

        protected override void Expire()
        {
            Destroy(this.gameObject);
        }
    }
}

