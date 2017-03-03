using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.PowerUp.ScoreBoost 
{
    public class ScoreBoost : AbstractPowerUp
    {

        void Awake() 
        {
            base.Awake();
        }

        protected override void ActivatePowerUp() 
        {
            GameManager.scoreMultiplier *= 2;
        }

        protected override void DeactivatePowerUp() 
        {
            if (GameManager.scoreMultiplier > 1) 
            {
                GameManager.scoreMultiplier = (GameManager.scoreMultiplier/2);
            }
            
        }

        protected override void UpdatePowerUp()
        {

        }

        protected override void Expire()
        {
            Destroy(this.gameObject);
        }
    }
}

