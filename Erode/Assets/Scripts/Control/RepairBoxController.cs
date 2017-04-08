using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HexGridGenerator;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class RepairBoxController: MonoBehaviour
    {
        private Dictionary<int, Tile> _collidingTiles = new Dictionary<int, Tile>();

        public Dictionary<int, Tile> CollidingTiles
        {
            get { return this._collidingTiles; }
        }

        //private PlayerController _playerController;

        void Awake()
        {
            //this._playerController = this.transform.parent.GetComponent<PlayerController>();
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Tile")
            {
                var tile = col.GetComponent<Tile>();
                Debug.Assert(tile != null);
                if (!this._collidingTiles.ContainsKey(tile.GetHashCode()))
                {
                    this._collidingTiles.Add(tile.GetHashCode(), tile);
                }
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.tag == "Tile")
            {
                //var tile = col.GetComponent<Tile>();
                //Debug.Assert(tile != null);
                //if (this._collidingTiles.ContainsKey(tile.GetHashCode()))
                //{
                    this._collidingTiles.Remove(col.GetComponent<Tile>().GetHashCode());
                //}
            }
        }
    }
}
