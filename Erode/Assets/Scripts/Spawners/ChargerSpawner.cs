using Assets.Scripts.HexGridGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    class ChargerSpawner : AbstractSpawner
    {
        public GameObject Charger;
        public GameObject Spawner;

        protected override void Spawn()
        {
            Tile tile = Grid.inst.GetRandomTile(true, false);
            Vector3 pos = tile.transform.position;
            Instantiate(Spawner, new Vector3(pos.x, pos.y + 0.5f, pos.z), (Quaternion.Euler(0, 0, 0)), _spawnObjectParent.transform);

            StartCoroutine(SpawnChargerDelayed(pos));
        }

        private IEnumerator SpawnChargerDelayed(Vector3 pos)
        {
            yield return new WaitForSeconds(1.5f);
            Instantiate(Charger, pos, (Quaternion.Euler(0, 0, 0)), _spawnObjectParent.transform);
        }
    }
}
