using UnityEngine;

namespace Assets.Obstacles.Comet
{
    public class CometSpawn : MonoBehaviour
    {
        public GameObject RedXComet;
        public GameObject[] Comet;
        public float SpawnTime = 5f;
        public float CometSpeed = 10f;
        //Temporaire, doit trouver le vrai rayon de la plateforme initial
        public float SpawnRadius = 15;

        private float _rayonPlateforme = 25;

        // Use this for initialization
        void Start()
        {
            this.InvokeRepeating("Spawn", 0, this.SpawnTime);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Spawn()
        {
            Vector3 startPos = this.DeterminePosition();
            Vector3 endPos = startPos;
            endPos.y -= 60;

            Vector3 direction = endPos - startPos;

            //Calcul de la position ou la comete va tomber, afin d'appeler la fonction qui va afficher le target.
            float yPoint = startPos.y;
            Vector3 posAtZero = startPos;

            Vector3 normDir = direction.normalized;

            while (yPoint > 0)
            {
                //Direction est négative , on l'additionne donc.
                yPoint = yPoint + normDir.y;
                posAtZero = posAtZero + normDir;
            }

            GameObject newComet = Instantiate(this.Comet[(int)Random.Range(0, this.Comet.Length - 0.1f)], startPos, (Quaternion.Euler(0, 0, 0)));
            newComet.GetComponent<CometController>().CometVelocity = newComet.GetComponent<Rigidbody>().velocity = direction.normalized * this.CometSpeed;

            this.ShowImpactPoint(posAtZero);
        }

        private Vector3 DeterminePosition()
        {
            float spawnAngle = UnityEngine.Random.Range(0, 359);
            Vector3 pos;

            pos.x = Random.Range(0.0f, this._rayonPlateforme) * Mathf.Sin(spawnAngle * Mathf.Deg2Rad);
            pos.z = Random.Range(0.0f, this._rayonPlateforme) * Mathf.Cos(spawnAngle * Mathf.Deg2Rad);
            pos.y = 30.0f;

            return pos;

        }

        private void ShowImpactPoint(Vector3 positionOnGrid) 
        { 
            //Prendre les coordonnées pour faire apparaitre un PNG en x et z.

            Vector3 posTarget = new Vector3(positionOnGrid.x, 0.5f, positionOnGrid.z);
            Instantiate(this.RedXComet, posTarget, Quaternion.Euler(90, 0, 0));

        }        

    }
}
