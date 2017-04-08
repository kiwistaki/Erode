using Assets.Obstacles.Asteroide;
using System;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    class AsteroidSpawner : AbstractSpawner
    {
        public float SpawnTime = 3.0f;
        public GameObject[] Asteroide;
        public float Radius = 35.0f;
        [Range(0.0f, 180.0f)]
        public float AngleVariation = 35.0f;
        public float AsteroidHeight = 1.5f;
        [Range(0.0f, 10.0f)]
        public float AsteroidSpeed = 4.0f;
        [Range(0.0f, 180.0f)]
        public float AsteroidAngularSpeed = 15.0f;

        protected override void Spawn()
        {
            float minDirectionAngle = (180.0f - this.AngleVariation) / 2.0f + 90.0f;
            float maxDirectionAngle = 270.0f - (180.0f - this.AngleVariation) / 2.0f;

            if (minDirectionAngle > maxDirectionAngle)
            {
                var tmp = maxDirectionAngle;
                maxDirectionAngle = minDirectionAngle;
                minDirectionAngle = tmp;
            }

            //L'angle utilisé pour faire spawner l'asteroide va être random, ainsi que la rotation de l'astéroide
            float spawnAngleSource = UnityEngine.Random.Range(0, 359);
            //Si l'angle entre les deux points est très basse, les chances qu'un astéroide passe au dessus de la plateforme est mince.
            float spawnAngleDest = UnityEngine.Random.Range(minDirectionAngle, maxDirectionAngle);
            if ((spawnAngleDest += spawnAngleSource) > 360)
            {
                spawnAngleDest -= 360;
            }

            Vector3 startPos = this.CalculatePosition(spawnAngleSource);
            Vector3 endPos = this.CalculatePosition(spawnAngleDest);
            Vector3 direction = endPos - startPos;

            GameObject newAst = Instantiate(this.Asteroide[(int)UnityEngine.Random.Range(0, this.Asteroide.Length - 0.1f)], startPos, Quaternion.Euler(0, 0, 0), _spawnObjectParent.transform);
            newAst.GetComponent<AsteroideController>().AsteroidVelocity = newAst.GetComponent<Rigidbody>().velocity = direction.normalized * this.AsteroidSpeed;
            var angularSpeed = Vector3.zero;
            angularSpeed.x = UnityEngine.Random.Range(0.0f, this.AsteroidAngularSpeed);
            angularSpeed.y = UnityEngine.Random.Range(0.0f, this.AsteroidAngularSpeed - angularSpeed.x);
            angularSpeed.z = this.AsteroidAngularSpeed - (angularSpeed.x + angularSpeed.y);
            newAst.GetComponent<AsteroideController>().AngularSpeed = angularSpeed * this.AsteroidSpeed;
        }

        private Vector3 CalculatePosition(float angle)
        {
            Vector3 pos = Vector3.zero;
            pos.x = this.Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            pos.z = this.Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            pos.y = this.AsteroidHeight;
            return pos;
        }
    }
}
