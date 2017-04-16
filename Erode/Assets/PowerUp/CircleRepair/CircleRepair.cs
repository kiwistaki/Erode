using Assets.Scripts.HexGridGenerator;
using Assets.Scripts.iTween;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PowerUp.CircleRepair
{
    class CircleRepair : AbstractPowerUp
    {
        public int RepairRadius = 15;

        private new void Awake()
        {
            base.Awake();
            this.gameObject.transform.position += new Vector3(0f, 0.3f, 0f);
            iTween.MoveBy(this.gameObject, iTween.Hash
                ( "name", "UpAndDown"
                ,"amount", this.transform.up * 0.3f
                , "time", 1.0f
                , "looptype", iTween.LoopType.pingPong
                , "easetype", iTween.EaseType.easeInOutSine
                ));          
        }

        protected override void ActivatePowerUp()
        {
            ResetTile();            
            List<List<Tile>> concentricCircles = Utils.GetConcentricCircles(this.gameObject.GetComponent<Tile>(), RepairRadius);
            foreach (List<Tile> circle in concentricCircles)
            {
                foreach (Tile tile in circle)
                {
                    tile.RepairFromPosition(this.gameObject.transform.position + new Vector3(0f, 2f, 0f), false);
                }
            }
        }

        protected override void DeactivatePowerUp()
        {
            Destroy(GetComponent<CircleRepair>());
        }

        protected override void Expire()
        {
            ResetTile();
            Destroy(GetComponent<CircleRepair>());
        }

        protected override void UpdatePowerUp()
        {            
        }

        private void ResetTile()
        {
            gameObject.GetComponent<MeshRenderer>().material.Lerp(gameObject.GetComponent<MeshRenderer>().material, Grid.inst.hexMaterial, 1f);
            foreach(Transform t in gameObject.transform)
                Destroy(t.gameObject);
            gameObject.GetComponent<MeshCollider>().isTrigger = false;
            gameObject.GetComponent<MeshCollider>().convex = false;
            gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            iTween.StopByName(this.gameObject, "UpAndDown");
        }
    }
}
