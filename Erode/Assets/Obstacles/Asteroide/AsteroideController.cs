using Assets.Scripts.Control;
using UnityEngine;

namespace Assets.Obstacles.Asteroide
{
    public class AsteroideController : MonoBehaviour
    {
        public float OutOfZoneMagnitude = 80.0f;

        public Vector3 AsteroidVelocity { get; set; }
        public Vector3 AngularSpeed { get; set; }

        private HammerController.HammerType _hitType;
        public HammerController.HammerType HitType
        {
            get { return this._hitType; }
            set
            {
                this._hitType = value;
                if (value == HammerController.HammerType.Quick)
                {
                    //give grace time only on a quick hit
                    this._graceTime = 0.6f;
                }
            }
        }

        private float _graceTime = 0.0f;

        void Awake()
        {
            this.transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
            this._hitType = HammerController.HammerType.Undefined;
        }

        void Update()
        {
            this.transform.position += this.AsteroidVelocity * Time.deltaTime;
            this.transform.Rotate(this.AngularSpeed * Time.deltaTime, Space.World);
            
            this._graceTime -= Time.deltaTime;

            this.CheckOutOfZone();
        }

        void OnTriggerEnter(Collider collider)
        {
            //Collisions need the velocity of the moving object, so we store it in the rigidbody temporarily
            this.GetComponent<Rigidbody>().velocity = this.AsteroidVelocity;
            switch (collider.tag)
            {
                case "Asteroid":
                    collider.GetComponent<AsteroideController>().CollisionBetweenAsteroids(this.gameObject);
                    break;

                case "Player":
                    //Dont trigger if player has hit the asteroid
                    if (this._graceTime <= 0.0f)
                    {
                        collider.GetComponent<PlayerController>().OnAsteroidCollision(this.gameObject);
                    }
                    break;

                case "Hunter":
                    if (HitType==HammerController.HammerType.Undefined)
                    {
                        collider.GetComponent<HunterController>().OnAsteroidCollision(this.gameObject);
                    }
                    else
                    {
                        collider.GetComponent<HunterController>().HitPoint = 0;
                    }
                    break;
                case "Charger":
                    if (HitType == HammerController.HammerType.Undefined)
                    {
                        collider.GetComponent<ChargerController>().OnAsteroidCollision(this.gameObject);
                    }
                    else
                    {
                        collider.GetComponent<ChargerController>().HitPoint = 0;
                    }
                    break;

                case "Hammer":
                    collider.GetComponent<HammerController>().OnAsteroidCollision(this.gameObject);
                    break;

                case "Shooter":
                    if (HitType == HammerController.HammerType.Undefined)
                    {
                        collider.GetComponent<ShooterController>().OnAsteroidCollision(this.gameObject);
                    }
                    else
                    {
                        collider.GetComponent<ShooterController>().HitPoint = 0;
                    }
                    break;
            }
        }

        void OnTriggerStay(Collider collider)
        {
            switch (collider.tag)
            {
                case "Player":
                    this.PushOther(collider);
                    break;

                case "Hunter":
                    this.PushOther(collider);
                    break;

                default:
                    break;
            }
        }

        private void PushOther(Collider collider)
        {
            var delta = collider.transform.position - this.transform.position;
            var squeeze = this.GetComponent<SphereCollider>().radius*2.0f/delta.magnitude;
            collider.transform.position += delta*Time.deltaTime*squeeze*squeeze*squeeze*squeeze;
        }

        private void CollisionBetweenAsteroids(GameObject asteroid)
        {
            switch (this.HitType)
            {
                case HammerController.HammerType.ChargedLow:
                    Destroy(asteroid.gameObject);
                    break;

                default:
                    //Saving current asteroid speed
                    var speed = this.AsteroidVelocity.magnitude;
                    //Computing impulse
                    Vector3 newDirection = (this.transform.position - asteroid.transform.position).normalized;
                    //Apply speed to impulse
                    this.AsteroidVelocity = newDirection * speed;
                    this.HitType = HammerController.HammerType.Undefined;
                    break;
            }
        }

        public void CollisionWithBlitz(GameObject player)
        {
            //Saving current asteroid speed
            var speed = this.AsteroidVelocity.magnitude;
            //Computing impulse
            var collisionImpulse = player.transform.position - this.transform.position;
            collisionImpulse = player.transform.forward;
            collisionImpulse.y = 0;
            //Apply speed to impulse
            this.AsteroidVelocity = collisionImpulse.normalized * speed;
        }

        public void CollisionWithQuickHammer(GameObject player)
        {

        }

        public void CollisionWithChargedHammer(GameObject player)
        {

        }

        private void CheckOutOfZone()
        {
            if (this.transform.position.magnitude > this.OutOfZoneMagnitude)
            {
                Destroy(this.gameObject);
            }
        }
    }
}

