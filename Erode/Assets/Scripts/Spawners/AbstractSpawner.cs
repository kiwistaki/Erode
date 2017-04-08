using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    public abstract class AbstractSpawner : MonoBehaviour
    {
        protected int _spawnIntensity = 0;
        protected int _spawnBaseNumber = 0;
        protected int _spawnBaseTime = 0;
        protected GameObject _spawnObjectParent = null;

        private List<float> _spawnTimes = new List<float>();


        private bool _isActive = false;
        private float _timer = 0f;

        public void Initialize(int intensity, int baseNumber, int baseTime, GameObject spawnObjectParent)
        {
            _spawnIntensity = intensity;
            _spawnBaseNumber = baseNumber;
            _spawnBaseTime = baseTime;
            _spawnObjectParent = spawnObjectParent;
            _isActive = false;
            _timer = 0f;
            _spawnTimes.Clear();
        }

        public void ActivateSpawner()
        {
            _isActive = true;
            InitializeSpawnTimes();
        }

        public virtual void DeactivateSpawner()
        {
            _isActive = false;
        }

        public bool IsActive()
        {
            return _isActive;
        }

        protected void ExecuteSpawn()
        {
            _timer += Utils.Utils.getRealDeltaTime();
            if(_spawnTimes.Count == 0) // no more spawns to do
            {
                InitializeSpawnTimes();
            }
            else if(_timer > _spawnTimes[0])
            {
                Spawn();
                _spawnTimes.RemoveAt(0);
            }
        }

        protected abstract void Spawn();

        private void Update()
        {
            if(_isActive)
                this.ExecuteSpawn();
        }

        private void InitializeSpawnTimes()
        {
            for (int i = 0; i < _spawnBaseNumber * _spawnIntensity; i++)
            {
                _spawnTimes.Add(Random.Range(0f, (float)_spawnBaseTime));
            }
            _spawnTimes.Sort();
            _timer = 0f;
        }     
    }
}
