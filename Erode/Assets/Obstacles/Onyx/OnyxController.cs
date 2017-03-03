using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Assets.Obstacles.Asteroide;
using Assets.Scripts.Control;
using Assets.Scripts.HexGridGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Obstacles.Onyx
{
    class OnyxController : MonoBehaviour
    {
        public float BallDelta = 0.1f;

        public bool AsteroidCollisionEnabled = false;

        public Vector3 Velocity { get; set; }

        public float Amplitude { get; set; }

        public int Ball { get; set; }

        public float Period { get; set; }

        public GameObject OnyxPrefab { get; set; }

        public GameObject ShadowTrail { get; set; }

        private float _period = 0.0f;

        void Awake()
        {
            this._period = 0.0f;
            this.transform.localScale = Random.Range(0.8f, 1.2f)*Vector3.one;
        }

        void Update()
        {
            if (this.BallDelta > 0.0f && (this.BallDelta -= Time.deltaTime) <= 0.0f && --this.Ball != 0)
            {
                //Spawn the next ball
                var startPos = this.transform.position;
                startPos.y = 0.0f;
                startPos -= this.Velocity*this._period;
                GameObject onyx = Instantiate(this.OnyxPrefab, startPos, Quaternion.Euler(0, 0, 0));
                var controller = onyx.GetComponent<OnyxController>();
                controller.Velocity = this.Velocity;
                controller.Amplitude = this.Amplitude;
                controller.Ball = this.Ball;
                controller.Period = this.Period;
                controller.OnyxPrefab = this.OnyxPrefab;
                controller.ShadowTrail = Instantiate(this.ShadowTrail, startPos, this.ShadowTrail.transform.rotation);
            }
            this._period += Time.deltaTime;

            //Onyx position
            var previousPos = this.transform.position;
            this.transform.position += this.Velocity * Time.deltaTime;
            var amp = Vector3.up * this.Amplitude * Mathf.Sin(this._period/this.Period * 2 * Mathf.PI);
            var pos = this.transform.position;
            pos.y = 0;
            this.transform.position = amp + pos;
            var dir = this.transform.position - previousPos;
            if (dir.magnitude > 0.0f)
            {
                this.transform.Rotate(dir, Time.deltaTime * 360.0f);
                this.transform.rotation = Quaternion.LookRotation(dir, this.transform.up);
            }

            //Trail position
            var previousPosT = this.ShadowTrail.transform.position;
            var posT = this.transform.position;
            posT += this.Velocity*0.5f;
            posT.y = 0;
            var ampT = Vector3.up * this.Amplitude * Mathf.Sin((this._period+0.5f) / this.Period * 2 * Mathf.PI);
            this.ShadowTrail.transform.position = ampT + posT;
        }

        void OnTriggerEnter(Collider collider)
        {
            switch (collider.tag)
            {
                case "Tile":
                    //Using the Erode function to destroy the tile.
                    Tile tile = collider.GetComponent<Tile>();
                    int tileHp = tile.Hp;
                    tile.Erode(tileHp);
                    tile.Hide();
                    break;

                case "Asteroid":
                    if(this.AsteroidCollisionEnabled)
                        this.AsteroidCollision(collider.gameObject);
                    break;

                case "Player":
                    collider.GetComponent<PlayerController>().OnCometCollision(this.gameObject);
                    break;

                case "Hunter":
                    collider.GetComponent<HunterAI>().HitPoint = 0;
                    break;
            }
        }

        private void AsteroidCollision(GameObject asteroid)
        {
            var direction = asteroid.transform.position - this.transform.position;
            direction.y = 0;
            asteroid.GetComponent<AsteroideController>().AsteroidVelocity = direction.normalized * asteroid.GetComponent<AsteroideController>().AsteroidVelocity.magnitude;
        }

        private void CheckOutOfBounds()
        {
            if(this.transform.position.magnitude >= 80)
                Destroy(this);
        }
    }
}
