using Assets.Scripts.Camera;
using Assets.Obstacles;
using UnityEngine;

public class SolarwindController : MonoBehaviour {

    public GameObject   SolarwindVFX;
    public float        LifeExpectancy = 5.0f;
    public float        GetTargetTime = 1.0f;
    public float        PlayerMultiplier = 0.4f;
    public float        Smoothing = 5f;

    private GameObject          _solarwind;
    private Vector3             _direction;
    private CameraController    _camera;


    void Awake()
    {
        this._direction = new Vector3(Random.Range(-1.0f,1.0f), 0, Random.Range(-1.0f, 1.0f));
        this._camera = GameObject.Find("MainCamera").GetComponent<CameraController>();
        this._solarwind = Instantiate(this.SolarwindVFX, new Vector3(this._camera.transform.position.x, this._camera.transform.position.y/2, this._camera.transform.position.z), Quaternion.identity);
    }

    void Start()
    {
        Die();
        Camera.main.GetComponent<WindController>().AddVector(_direction, LifeExpectancy); 
    }

    void Update()
    {
        this._solarwind.transform.position = Vector3.Lerp(new Vector3(this._camera.transform.position.x, this._camera.transform.position.y / 2, this._camera.transform.position.z), this._solarwind.transform.position, Smoothing * Time.deltaTime);
    }


    void Die()
    {
        Destroy(this._solarwind.gameObject, LifeExpectancy);
        Destroy(this.gameObject, LifeExpectancy);
    }
}
