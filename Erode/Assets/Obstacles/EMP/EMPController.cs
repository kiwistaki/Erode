using Assets.Scripts.Control;
using Assets.Scripts.iTween;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Obstacles.EMP
{
    public class EMPController : MonoBehaviour
    {

        public GameObject EMPExplosion;
        public GameObject HitIndicatorPrefab;
        public float TimeBeforeExplosion = 5f;
        public int WaveRadius = 30;

        private float _timer = 0.0f, _explosionDuration = 3f;
        private bool _duringExplosion = false;
        private GameObject _hitIndicator;
        private bool _isTutorialActive = false;

        private ScoreManager _scoreManager;

        public void ShowHitTutorial()
        {
            _isTutorialActive = true;
        }

        // Use this for initialization
        void Start()
        {
            _scoreManager = GameObject.Find("MainCamera").GetComponent<ScoreManager>();
            Canvas tutorialCanvas = GameObject.Find("TutorialCanvas").GetComponent<Canvas>();
            _hitIndicator = Instantiate(HitIndicatorPrefab, tutorialCanvas.transform, false);
            _hitIndicator.transform.localPosition = Utils.GetScreenPosition(transform.position, tutorialCanvas, UnityEngine.Camera.main);
            _hitIndicator.GetComponent<HitIndicatorController>().SetTarget(this.gameObject);
            _hitIndicator.SetActive(false);
            
        }

        // Update is called once per frame
        void Update()
        {
            if (_hitIndicator != null)
            {
                if (_isTutorialActive)
                    _hitIndicator.SetActive(true);
                else if (_hitIndicator.activeSelf)
                    _hitIndicator.SetActive(false);
            }

            if (!_duringExplosion)
            {
                _timer += Time.deltaTime;
                if (_timer >= TimeBeforeExplosion)
                {
                    Destroy(gameObject);
                    _timer = 0.0f;
                }
            }
        }

        void TriggerExplosion()
        {
            if(_hitIndicator != null)
                Destroy(_hitIndicator.gameObject);
            _duringExplosion = true;
            GetComponent<ParticleSystem>().Stop();
            StartCoroutine(ProcessExplosion());
        }

        IEnumerator ProcessExplosion()
        {
            yield return new WaitWhile(() => !IsPulseStopped());
            Instantiate(EMPExplosion, transform.position, Quaternion.Euler(0, 0, 0));
            Utils.StartTileWave(Utils.GetClosestTile(gameObject.transform.position), WaveRadius);
            yield return new WaitForSeconds(_explosionDuration);

            Destroy(gameObject);
        }

        bool IsPulseStopped()
        {
            return GetComponent<ParticleSystem>().isStopped;
        }

        private void OnTriggerEnter(Collider other)
        {
            switch(other.tag)
            {
                case "Hammer":
                    GameObject.Find("Horatio").GetComponent<PlayerController>().PlayHitEffect(other.transform.position);
                    _scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.EMP, this.transform.parent.name);
                    _scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.EMP, this.transform.position);
                    TriggerExplosion();
                    Destroy(gameObject, 1f);
                    break;
                case "Onyx":                    
                    GameObject.Find("Horatio").GetComponent<PlayerController>().PlayHitEffect(other.transform.position);
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
