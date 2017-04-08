using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    class SolarwindSpawner : AbstractSpawner
    {
        public GameObject Solarwind;

        protected override void Spawn()
        {
            // Instantiation of new gameobject
            Instantiate(Solarwind, new Vector3(0, 0, 0), (Quaternion.Euler(0, 0, 0)), _spawnObjectParent.transform);
        }
    }
}
