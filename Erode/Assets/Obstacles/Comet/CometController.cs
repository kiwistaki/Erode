using UnityEngine;
using Assets.Scripts.Control;
using Assets.Obstacles.Asteroide;
using Assets.Scripts.HexGridGenerator;
using MirzaBeig.ParticleSystems;

namespace Assets.Obstacles.Comet
{
    public class CometController : MonoBehaviour
    {
        public GameObject TrailVfx;
        public GameObject CometExplosionVfx;
        public Vector3 CometVelocity { get; set; }
        public float OutOfZoneMagnitude = 80f;
        private float AngularSpeed = 90.0f;
        private GameObject _trail;
        private bool _firstCollision = true;

        void Awake()
        {
            this.transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
            this._trail = Instantiate(this.TrailVfx, this.transform.position, Quaternion.identity);
        }

        // Update is called once per frame
        void Update()
        {
            this.transform.position += this.CometVelocity * Time.deltaTime;
            this.transform.Rotate(this.CometVelocity, this.AngularSpeed * Time.deltaTime, Space.World);
            this._trail.transform.position = this.transform.position;
            this._trail.transform.rotation = Quaternion.LookRotation(this.CometVelocity.normalized);
            this.CheckOutOfZone();
            
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
                    //Vfx
                    if (this._firstCollision && this.transform.position.y - 2.0f < 0)
                    {
                        var pos = collider.transform.position + this.CometVelocity.normalized;
                        pos.y = 0.5f;
                        var badaboom = Instantiate(this.CometExplosionVfx, pos, Quaternion.identity).GetComponent<ParticleSystems>();
                        badaboom.setPlaybackSpeed(0.8f);
                        badaboom.simulate(.1f);
                        badaboom.play();
                        this.GetComponent<AudioSource>().Play();

                        this._firstCollision = false;
                    }
                    //Potentiellement ajouter qqch qui fait modifier la vitesse a laquelle la tile tombe.
                    break;

                case "Player":
                    collider.GetComponent<PlayerController>().OnCometCollision(this.gameObject);
                    break;
                case "Charger":
                    collider.GetComponent<ChargerController>().OnCometCollision(this.gameObject);
                    break;

                case "Shooter":
                    collider.GetComponent<ShooterController>().OnCometCollision(this.gameObject);
                    break;

                case "Target":
                    GameObject target = collider.gameObject;
                    Destroy(target);
                    break;
            }
        }

        void CheckOutOfZone()
        {
            if (this.transform.position.magnitude > this.OutOfZoneMagnitude)
            {
                Destroy(this.gameObject);
                Destroy(this._trail.gameObject);
            }
        }
    }
}
