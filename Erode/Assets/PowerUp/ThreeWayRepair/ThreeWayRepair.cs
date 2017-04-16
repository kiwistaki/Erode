using Assets.Scripts.HexGridGenerator;
using Assets.Scripts.iTween;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.PowerUp.ThreeWayRepair
{
    class ThreeWayRepair : AbstractPowerUp
    {
        public int RepairRadius = 15;

        private List<Tile> _tilesInRepairPattern = new List<Tile>();

        private new void Awake()
        {
            base.Awake();
            this.gameObject.transform.position += new Vector3(0f, 0.3f, 0f);
            iTween.MoveBy(this.gameObject, iTween.Hash
                ("name", "UpAndDown"
                , "amount", this.transform.up * 0.3f
                , "time", 1.0f
                , "looptype", iTween.LoopType.pingPong
                , "easetype", iTween.EaseType.easeInOutSine
                ));
        }

        protected override void ActivatePowerUp()
        {
            ResetTile();
            // Algorithm to get all the tiles in a three-way pattern, starting from this.gameObject
            CubeIndex idx = this.gameObject.GetComponent<Tile>().Index;
            int invertPattern = 1; // 1 for false, -1 for true
            if (UnityEngine.Random.Range(0, 1) < 0.5f) invertPattern = -1;
            SetTilesXWay(idx, invertPattern);
            SetTilesYWay(idx, invertPattern);
            SetTilesZWay(idx, invertPattern);
            
            // Repair all these tiles and their neighbors
            foreach(Tile t in _tilesInRepairPattern)
            {
                t.RepairFromPosition(this.gameObject.transform.position, false);
                foreach (Tile n in t.Neighbours)
                    n.RepairFromPosition(this.gameObject.transform.position, false);
            }            
        }

        protected override void DeactivatePowerUp()
        {
            Destroy(GetComponent<ThreeWayRepair>());
        }

        protected override void Expire()
        {
            ResetTile();
            Destroy(GetComponent<ThreeWayRepair>());
        }

        protected override void UpdatePowerUp()
        {
        }

        private void ResetTile()
        {
            gameObject.GetComponent<MeshRenderer>().material.Lerp(gameObject.GetComponent<MeshRenderer>().material, Grid.inst.hexMaterial, 1f);
            foreach (Transform t in gameObject.transform)
                Destroy(t.gameObject);
            gameObject.GetComponent<MeshCollider>().isTrigger = false;
            gameObject.GetComponent<MeshCollider>().convex = false;
            gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            iTween.StopByName(this.gameObject, "UpAndDown");
        }

        private void SetTilesXWay(CubeIndex idx, int invert)
        {
            int offset = 0;
            Tile tileToRepair = null;
            while (offset < 15)
            {
                tileToRepair = Grid.inst.TileAt(idx.x, idx.y - (invert * offset), idx.z + (invert * offset));
                if (tileToRepair != null)
                    _tilesInRepairPattern.Add(tileToRepair);
                else
                    break;
                offset++;
            }
        }

        private void SetTilesYWay(CubeIndex idx, int invert)
        {
            int offset = 0;
            Tile tileToRepair = null;
            while (offset < 15)
            {
                tileToRepair = Grid.inst.TileAt(idx.x + (invert*offset), idx.y, idx.z - (invert*offset));
                if (tileToRepair != null)
                    _tilesInRepairPattern.Add(tileToRepair);
                else
                    break;
                offset++;
            }
        }

        private void SetTilesZWay(CubeIndex idx, int invert)
        {
            int offset = 0;
            Tile tileToRepair = null;
            while (offset < 15)
            {
                tileToRepair = Grid.inst.TileAt(idx.x - (invert * offset), idx.y + (invert * offset), idx.z);
                if (tileToRepair != null)
                    _tilesInRepairPattern.Add(tileToRepair);
                else
                    break;
                offset++;
            }
        }
    }
}
