using Assets.Scripts.HexGridGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Obstacles.EMP
{
    public class EMPSpawn : MonoBehaviour
    {

        public GameObject EMPPulse;
        public float SpawnTime = 10f;

        // Use this for initialization
        void Start()
        {
            this.InvokeRepeating("Spawn", 0, this.SpawnTime);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Spawn()
        {
            Vector3 pos = Grid.inst.GetRandomBorderTile().transform.position;
            pos = pos + new Vector3(0f, 1.5f, 0f);
            Instantiate(this.EMPPulse, pos, Quaternion.Euler(0, 0, 0));
        }
    }
}
