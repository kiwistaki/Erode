using Assets.Scripts.HexGridGenerator;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets.PowerUp;

namespace Assets.PowerUp
{
    public class PowerUpSpawner : MonoBehaviour
    {
        public GameObject SuperSpeedPrefab = null;
        public GameObject ScoreMultiplierPrefab = null;
        public GameObject SlowMotionPrefab = null;
        public GameObject AmmoPrefab = null;

        public float SpawnInterval = 5.0f;

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
                var type = (AbstractPowerUp.PowerUpType)Random.Range(0.0f, (float)AbstractPowerUp.PowerUpType.PowerUpCount - 0.1f);
                switch (type)
                {
                    case AbstractPowerUp.PowerUpType.SuperSpeed:
                        Instantiate(this.SuperSpeedPrefab, position, Quaternion.identity);
                        break;
                    case AbstractPowerUp.PowerUpType.ScoreMultiplier:
                        Instantiate(this.ScoreMultiplierPrefab, position, Quaternion.identity);
                        break;
                    case AbstractPowerUp.PowerUpType.SlowMotion:
                        Instantiate(this.SlowMotionPrefab, position, Quaternion.identity);
                        break;
                    case AbstractPowerUp.PowerUpType.Ammo:
                        Instantiate(this.AmmoPrefab, position, Quaternion.identity);
                        break;
                    default:
                        throw new UnityException("PowerUpSpawner::SpawnPowerUp: " + type.ToString() + " IS NOT IMPLEMENTED");
                }
            }
        }
    }
}
