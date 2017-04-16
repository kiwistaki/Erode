using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitIndicatorController : MonoBehaviour {

    private Image _hitIndicatorImage;
    private float _blinkTimer = 0f;
    private GameObject _target;
    private Canvas _tutorialCanvas;

	// Use this for initialization
	void Start ()
    {
        _hitIndicatorImage = GetComponent<Image>();
        _tutorialCanvas = GameObject.Find("TutorialCanvas").GetComponent<Canvas>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(_target != null)
        {
            this.gameObject.transform.localPosition = Utils.GetScreenPosition(_target.transform.position, _tutorialCanvas, Camera.main);
        }
        else
        {
            Destroy(this.gameObject);
        }

        _blinkTimer += Utils.getRealDeltaTime();
        float ms = _blinkTimer - Mathf.Floor(_blinkTimer);
        if (ms < 0.25 || (ms > 0.5 && ms < 0.75))
            _hitIndicatorImage.enabled = false;
        else
            _hitIndicatorImage.enabled = true;
	}

    public void SetTarget(GameObject target)
    {
        _target = target;
    }
}
