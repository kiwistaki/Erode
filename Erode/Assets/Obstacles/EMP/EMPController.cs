using Assets.Scripts.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Obstacles.EMP
{
    public class EMPController : MonoBehaviour
    {

        public GameObject EMPExplosion;
        public float TimeBeforeExplosion = 5f;

        private float _timer = 0.0f, _explosionDuration = 3f;
        private bool _duringExplosion = false;

        private ScoreManager _scoreManager;

        // Use this for initialization
        void Start()
        {
            this._scoreManager = GameObject.Find("MainCamera").GetComponent<ScoreManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!_duringExplosion)
            {
                _timer += Time.deltaTime;
                if (_timer >= TimeBeforeExplosion)
                {
                    TriggerExplosion();
                    _timer = 0.0f;
                }
            }
        }

        void TriggerExplosion()
        {
            _duringExplosion = true;
            this.GetComponent<ParticleSystem>().Stop();
            StartCoroutine(ProcessExplosion());
        }

        IEnumerator ProcessExplosion()
        {
            yield return new WaitWhile(() => !IsPulseStopped());
            Instantiate(this.EMPExplosion, this.transform.position, Quaternion.Euler(0, 0, 0));
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().OnEMPExplosion();
            yield return new WaitForSeconds(_explosionDuration);

            Destroy(this.gameObject);
        }

        bool IsPulseStopped()
        {
            return this.GetComponent<ParticleSystem>().isStopped;
        }

        private void OnTriggerEnter(Collider other)
        {
            switch(other.tag)
            {
                case "Hammer":
                    GameObject.Find("Horatio").GetComponent<PlayerController>().PlayHitEffect(other.transform.position);
                    _scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.EMP, this.transform.parent.name);
                    _scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.EMP, this.transform.position);
                    Destroy(this.gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
