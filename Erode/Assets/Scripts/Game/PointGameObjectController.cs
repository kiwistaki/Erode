using Assets.Scripts.iTween;
using UnityEngine;

public class PointGameObjectController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.gameObject.transform.rotation = GameObject.Find("MainCamera").GetComponent<Transform>().rotation;

        Vector3 moveTo = this.gameObject.transform.position + new Vector3(0, 2, 0);
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "name", "Floating",
            "position", moveTo,
            "time", 2f,
            "looptype", iTween.LoopType.none,
            "easetype", iTween.EaseType.easeInBack,
            "ignoretimescale", false,
            "oncomplete", "Expire"
            ));
        iTween.FadeTo(this.gameObject, iTween.Hash(
            "name", "Floating",
            "alpha", 0,
            "time", 2f,
            "looptype", iTween.LoopType.none,
            "easetype", iTween.EaseType.easeInExpo
            ));
	}

    private void Expire()
    {
        Destroy(this.gameObject);
    }
}
