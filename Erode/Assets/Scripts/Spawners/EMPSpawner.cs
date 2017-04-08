using Assets.Scripts.Control;
using Assets.Scripts.HexGridGenerator;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    class EMPSpawner : AbstractSpawner
    {
        public GameObject EMPPulse;
        public PlayerController PlayerController;
        public int SpawnDistanceFromPlayer = 10;

        protected override void Spawn()
        {
            // Get Tile under player
            RaycastHit hitInfo;
            LayerMask mask = (1 << 8);
            Tile hittedTile = null;
            if (Physics.Raycast(new Ray(PlayerController.transform.position, Vector3.down), out hitInfo, Mathf.Infinity, mask))
            {
                hittedTile = hitInfo.collider.gameObject.GetComponent<Tile>();
            }

            Vector3 pos = new Vector3(0, 0, 0);
            if (hittedTile != null)
            {
                List<Tile> tiles = new List<Tile>();
                foreach (KeyValuePair<string, Tile> pair in Grid.inst.Tiles)
                {
                    int distance = Grid.inst.Distance(pair.Value, hittedTile);
                    if (distance == SpawnDistanceFromPlayer)
                        tiles.Add(pair.Value);
                }
                Tile spawnTile = tiles[new System.Random().Next(tiles.Count)];
                pos = spawnTile.transform.position;
                pos.y += 1.5f;
            }
            else
            {
                pos = Grid.inst.GetRandomBorderTile().transform.position;
                pos = pos + new Vector3(0f, 1.5f, 0f);
                
            }
            Instantiate(this.EMPPulse, pos, Quaternion.Euler(0, 0, 0), _spawnObjectParent.transform);
        }
    }
}
