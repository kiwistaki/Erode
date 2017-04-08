using Assets.Scripts.Utils;
using System.Collections;
using UnityEngine;

namespace Assets.Enemies
{
    class HealthBarController : MonoBehaviour
    {  
        public GameObject HealthBarPrefab;

        private Canvas _healthBarCanvas;
        private GameObject _healthBar;
        private ProgressBarPro _healthBarScript;
        private float _timerToHideBar = 1.5f;

        public void Start()
        {
            _healthBarCanvas = GameObject.Find("HealthBarsCanvas").GetComponent<Canvas>();
            _healthBar = Instantiate(this.HealthBarPrefab, this._healthBarCanvas.transform, false);
            _healthBarScript = _healthBar.GetComponent<ProgressBarPro>();
            _healthBar.transform.localScale = new Vector3(.12f, .12f, .12f);
            Vector3 pos = this.transform.position + new Vector3(0f, 1.5f, 0f);
            _healthBar.transform.localPosition = Utils.GetScreenPosition(pos, _healthBarCanvas, Camera.main);
            _healthBar.SetActive(false);
        }

        public void Update()
        {
            if(_timerToHideBar < 0)
            {
                _timerToHideBar = 1.5f;
                _healthBar.SetActive(false);
            }
            if (_healthBar.activeSelf)
            {
                _timerToHideBar -= Utils.getRealDeltaTime();
                Vector3 pos = this.transform.position + new Vector3(0f, 1.5f, 0f);
                _healthBar.transform.localPosition = Utils.GetScreenPosition(pos, _healthBarCanvas, Camera.main);
            }
        }

        public void OnDestroy()
        {
            Destroy(_healthBar);
        }

        public void ChangeHealth(float currentHealth, float maxHealth)
        {
            _healthBarScript.SetValue(currentHealth / maxHealth);
        }

        public void ShowHealthBar()
        {
            _healthBar.SetActive(true);
            _timerToHideBar = 1.5f;
        }
    }
}
