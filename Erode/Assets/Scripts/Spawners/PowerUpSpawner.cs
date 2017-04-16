using Assets.PowerUp.CircleRepair;
using Assets.PowerUp.ThreeWayRepair;
using Assets.Scripts.HexGridGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    class PowerUpSpawner : AbstractSpawner
    {
        public GameObject SuperSpeedPrefab;
        public GameObject ScoreMultiplierPrefab;
        public GameObject SlowMotionPrefab;

        private List<PowerUpType> _enabledPowerUps = new List<PowerUpType>();
        private System.Random r = new System.Random();

        public enum PowerUpType
        {
            SuperSpeed,
            ScoreMultiplier,
            SlowMotion,
            CircleRepair,
            ThreeWayRepair,
            PowerUpCount
        }

        public void EnablePowerUp(PowerUpType type)
        {
            _enabledPowerUps.Add(type);
        }

        protected override void Spawn()
        {
            if (_enabledPowerUps.Count > 0)
            {
                var tile = Grid.inst.GetRandomTile(true, true);
                if (tile != null)
                {
                    var position = tile.transform.position + tile.transform.up;
                    var type = _enabledPowerUps[r.Next(_enabledPowerUps.Count)];
                    switch (type)
                    {
                        case PowerUpType.SuperSpeed:
                            Instantiate(this.SuperSpeedPrefab, position, Quaternion.identity, _spawnObjectParent.transform);
                            break;
                        case PowerUpType.ScoreMultiplier:
                            Instantiate(this.ScoreMultiplierPrefab, position, Quaternion.identity, _spawnObjectParent.transform);
                            break;
                        case PowerUpType.SlowMotion:
                            Instantiate(this.SlowMotionPrefab, position, Quaternion.identity, _spawnObjectParent.transform);
                            break;
                        case PowerUpType.CircleRepair:
                            tile.GetComponent<MeshRenderer>().material.Lerp(tile.GetComponent<MeshRenderer>().material, Grid.inst.ShinyTileMaterial, 0.4f);
                            Instantiate(Grid.inst.ShinyTileVFXPrefab1, tile.transform.position + new Vector3(0f, 1.5f, 0f), Quaternion.Euler(0f, 0f, 0f), tile.transform);
                            tile.gameObject.AddComponent<CircleRepair>();
                            tile.GetComponent<MeshCollider>().convex = true;
                            tile.GetComponent<MeshCollider>().isTrigger = true;
                            break;
                        case PowerUpType.ThreeWayRepair:
                            tile.GetComponent<MeshRenderer>().material.Lerp(tile.GetComponent<MeshRenderer>().material, Grid.inst.ShinyTileMaterial, 0.4f);
                            Instantiate(Grid.inst.ShinyTileVFXPrefab2, tile.transform.position + new Vector3(0f, 1.5f, 0f), Quaternion.Euler(0f, 0f, 0f), tile.transform);
                            tile.gameObject.AddComponent<ThreeWayRepair>();
                            tile.GetComponent<MeshCollider>().convex = true;
                            tile.GetComponent<MeshCollider>().isTrigger = true;
                            break;
                        default:
                            throw new UnityException("PowerUpSpawner::SpawnPowerUp: " + type.ToString() + " IS NOT IMPLEMENTED");
                    }
                }
            }
        }

        public override void DeactivateSpawner()
        {
            base.DeactivateSpawner();
            _enabledPowerUps.Clear();
        }
    }
}
