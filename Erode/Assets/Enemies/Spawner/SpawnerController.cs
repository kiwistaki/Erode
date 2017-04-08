using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour {

    public GameObject SpawnerVFX;
    public float LifeExpectancy;

    private GameObject _spawnerVFX;

	void Start () {
        Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y + 1.15f, this.transform.position.z);
        this._spawnerVFX = Instantiate(SpawnerVFX, pos, Quaternion.identity, this.transform);
        ClosePortal();
	}

    void ClosePortal()
    {
        Destroy(this._spawnerVFX, LifeExpectancy);
        Destroy(this.gameObject, LifeExpectancy);
    }
}
