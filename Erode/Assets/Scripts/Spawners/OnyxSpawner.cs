using Assets.Obstacles.Onyx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Spawners
{
    class OnyxSpawner : AbstractSpawner
    {
        public GameObject OnyxPrefab = null;
        public GameObject OnyxTrailPrefab = null;
        public float Height = 0.0f;
        public float Radius = 35.0f;
        [Range(0.0f, 180.0f)]
        public float AngleVariation = 35.0f;

        public float OnyxAmplitude = 7.0f;
        public float OnyxSpeed = 1.5f;
        public float OnyxPeriod = 3.0f;
        public int OnyxBalls = 15;

        protected override void Spawn()
        {
            float minDirectionAngle = (180.0f - this.AngleVariation) / 2.0f + 90.0f, 
                maxDirectionAngle = 270.0f - (180.0f - this.AngleVariation) / 2.0f;

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

            GameObject onyx = Instantiate(this.OnyxPrefab, startPos, Quaternion.Euler(0, 0, 0), _spawnObjectParent.transform);
            onyx.transform.localScale = Vector3.one * 1.5f;
            var controller = onyx.GetComponent<OnyxController>();
            controller.Velocity = direction.normalized * this.OnyxSpeed;
            controller.Amplitude = this.OnyxAmplitude;
            controller.Period = this.OnyxPeriod;
            controller.Ball = this.OnyxBalls;
            controller.OnyxPrefab = this.OnyxPrefab;
            controller.ShadowTrail = Instantiate(this.OnyxTrailPrefab, startPos, this.OnyxTrailPrefab.transform.rotation, _spawnObjectParent.transform);
        }

        Vector3 CalculatePosition(float angle)
        {
            Vector3 pos = Vector3.zero;
            pos.x = this.Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            pos.z = this.Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            pos.y = this.Height;
            return pos;
        }
    }
}
