using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Control;
using Assets.Obstacles.Asteroide;

public class BarriereController : MonoBehaviour {

	// Use this for initialization
    public Vector3 BarriereVelocity { get; set; }
    public Vector3 AngularSpeed { get; set; }

    public float OutOfZoneMagnitude = 80.0f;
    

	void Start () {
		
	}
	
	// Update is called once per frame
    void Update()
    {
        this.transform.position += this.BarriereVelocity * Time.deltaTime;
        this.transform.Rotate(this.AngularSpeed * Time.deltaTime, Space.World);


        this.CheckOutOfZone();
    }


    void Awake()
    {
        //LUL
        this.transform.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
    }

    void OnTriggerEnter(Collider collider)
    {
        //Collisions need the velocity of the moving object, so we store it in the rigidbody temporarily
        this.GetComponent<Rigidbody>().velocity = this.BarriereVelocity;

        switch (collider.tag)
        {
            case "Asteroid":

                //gameObject.GetComponent<MeshCollider>().enabled = false;
                Vector3 collisionDirection = applyCollisionEffects(collider);
                Vector3 fixedDirection =  new Vector3(collisionDirection.x, 0, collisionDirection.z); 
                float magnitude = collider.GetComponent<AsteroideController>().AsteroidVelocity.magnitude;
                collider.GetComponent<AsteroideController>().AsteroidVelocity = fixedDirection.normalized * magnitude;
                break;

            case "Comet":
                {
                    Destroy(this.gameObject);
                }
                break;
            case "Player":
            case "Hunter":
            case "Charger":
                {
                    PushOther(collider);
                }
                break;
          
        }
    }

    /*void OnTriggerExit(Collider collider) 
    {
         switch (collider.tag)
         {
             case "Asteroid":

             //gameObject.GetComponent<MeshCollider>().enabled = true;
             break;

             default:
             break;
         }

        
    }*/

    private void CheckOutOfZone()
    {
        if (this.transform.position.magnitude > this.OutOfZoneMagnitude)
        {
            Destroy(this.gameObject);
        }
    }

    Vector3 applyCollisionEffects(Collider col) 
    {        
        Vector3 newDirection;
              
        Vector3 normale = this.gameObject.transform.right;
        Vector3 astVelocity = col.GetComponent<AsteroideController>().AsteroidVelocity;
        Vector3 projection = Vector3.Project(-astVelocity, normale);


        var x2 = normale.x * projection.x;
        var z2 = normale.z * projection.z;
        bool isGoodNormale = true;
        
        if (!(x2 >= 0.0f && z2 >= 0.0f)) 
        {
            isGoodNormale = false;
        }

        if (!isGoodNormale)
        {
           normale = -this.gameObject.transform.right;
        }

        Vector3 reflect = Vector3.Reflect(astVelocity, normale);
        newDirection = reflect.normalized;
        
        col.transform.TransformDirection(newDirection);

        return newDirection;
    }

    /*void OnTriggerStay(Collider collider)
    {
        switch (collider.tag)
        {

            case "Asteroid":
                Vector3 collisionDirection = applyCollisionEffects(collider);
                Vector3 fixedDirection =  new Vector3(collisionDirection.x, 0, collisionDirection.z); 
                float magnitude = collider.GetComponent<AsteroideController>().AsteroidVelocity.magnitude;
                collider.GetComponent<AsteroideController>().AsteroidVelocity = fixedDirection.normalized * magnitude;
                break;
            
            default:
                break;
        }
    }*/

    private void PushOther(Collider collider)
    {
        this.transform.position += (this.transform.position - collider.transform.position) * Time.deltaTime;
    }
}
