using UnityEngine;

namespace Assets.Obstacles.Asteroide
{
    public class AsteroideSpawn : MonoBehaviour {

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

        private float _minDirectionAngle;
        private float _maxDirectionAngle;


        void Awake()
        {
            this._minDirectionAngle = (180.0f-this.AngleVariation)/2.0f + 90.0f;
            this._maxDirectionAngle = 270.0f - (180.0f-this.AngleVariation)/2.0f;
        }

        void Start()
        {
            if (this._minDirectionAngle > this._maxDirectionAngle)
            {
                var tmp = this._maxDirectionAngle;
                this._maxDirectionAngle = this._minDirectionAngle;
                this._minDirectionAngle = tmp;
            }
            this.InvokeRepeating("Spawn", 0, this.SpawnTime);
        }
	
        // Update is called once per frame
        void Update()
        {
		
        }

        //L'astéroide va spawn a un angle random et une distance fixe de la plateforme
        void Spawn() 
        {
            //L'angle utilisé pour faire spawner l'asteroide va être random, ainsi que la rotation de l'astéroide
            float spawnAngleSource = Random.Range(0, 359);
            //Si l'angle entre les deux points est très basse, les chances qu'un astéroide passe au dessus de la plateforme est mince.
            float spawnAngleDest = Random.Range(this._minDirectionAngle, this._maxDirectionAngle);
            if ((spawnAngleDest += spawnAngleSource) > 360)
            {
                spawnAngleDest -= 360;
            }

            Vector3 startPos = this.CalculatePosition(spawnAngleSource);
            Vector3 endPos = this.CalculatePosition(spawnAngleDest);
            Vector3 direction = endPos - startPos;

            GameObject newAst = Instantiate(this.Asteroide[(int)Random.Range(0,this.Asteroide.Length-0.1f)], startPos, Quaternion.Euler(0,0,0));
            newAst.GetComponent<AsteroideController>().AsteroidVelocity = newAst.GetComponent<Rigidbody>().velocity = direction.normalized * this.AsteroidSpeed;
            var angularSpeed = Vector3.zero;
            angularSpeed.x = Random.Range(0.0f, this.AsteroidAngularSpeed);
            angularSpeed.y = Random.Range(0.0f, this.AsteroidAngularSpeed - angularSpeed.x);
            angularSpeed.z = this.AsteroidAngularSpeed - (angularSpeed.x + angularSpeed.y);
            newAst.GetComponent<AsteroideController>().AngularSpeed = angularSpeed * this.AsteroidSpeed;
        }

        Vector3 CalculatePosition(float angle)
        {
            Vector3 pos = Vector3.zero;
            pos.x = this.Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            pos.z = this.Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            pos.y = this.AsteroidHeight;
            return pos;
        }
    }
}
