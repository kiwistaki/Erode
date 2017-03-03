using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.HexGridGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.PowerUp
{
    public class PowerUpSpawner : MonoBehaviour
    {
        public GameObject SuperSpeedPrefab = null;
        public GameObject ScoreMultiplierPrefab = null;
        public GameObject SlowMotionPrefab = null;

        public float SpawnInterval = 5.0f;

        public enum PowerUpType
        {
            SuperSpeed,
            ScoreMultiplier,
            SlowMotion,
            PowerUpCount
        }

        private float _timer = 0.0f;

        public void Update()
        {
            if ((this._timer += Time.deltaTime) >= this.SpawnInterval)
            {
                this._timer = 0.0f;
                this.SpawnPowerUp();
            }
        }

        private void SpawnPowerUp()
        {
            var tile = Grid.inst.GetRandomTile(true, true);
            if (tile != null)
            {
                var position = tile.transform.position + tile.transform.up;
                var type = (PowerUpType)Random.Range(0.0f, (float)PowerUpType.PowerUpCount - 0.1f);
                GameObject powerUp;
                switch (type)
                {
                    case PowerUpType.SuperSpeed:
                        powerUp = Instantiate(this.SuperSpeedPrefab, position, Quaternion.identity);
                        break;
                    case PowerUpType.ScoreMultiplier:
                        powerUp = Instantiate(this.ScoreMultiplierPrefab, position, Quaternion.identity);
                        break;
                    case PowerUpType.SlowMotion:
                        powerUp = Instantiate(this.SlowMotionPrefab, position, Quaternion.identity);
                        break;
                    default:
                        throw new UnityException("PowerUpSpawner::SpawnPowerUp: " + type.ToString() + " IS NOT IMPLEMENTED");
                }
            }
        }
    }
}
