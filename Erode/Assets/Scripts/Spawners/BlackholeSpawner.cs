using Assets.Scripts.HexGridGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    class BlackholeSpawner : AbstractSpawner
    {
        public GameObject Blackhole;

        protected override void Spawn()
        {
            //getting one tile's position from the borderHexes
            Tile tile = Grid.inst.GetRandomBorderTile();
            Vector3 pos = new Vector3(tile.transform.position.x, 1, tile.transform.position.z);

            // Translate the blackhole off the tile a little bit on the X-axis
            if (pos.x > 0)
                pos.x += 1;
            else
                pos.x -= 1;
            // Translate the blackhole off the tile a little bit on the Z-axis
            if (pos.z > 0)
                pos.z += 1;
            else
                pos.z -= 1;

            // Instiation of new gameobject
            Instantiate(Blackhole, pos, (Quaternion.Euler(0, 0, 0)), _spawnObjectParent.transform);
        }
    }
}
