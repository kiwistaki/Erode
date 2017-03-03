using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class CameraController : MonoBehaviour {

        public Transform target;
        public float smoothing = 5f;

        Vector3 offset;

        void Start()
        {
            this.offset = this.transform.position - this.target.position;
        }

        void Update()
        {
            Vector3 targetCamPos = this.target.position + this.offset;
            this.transform.position = Vector3.Lerp(this.transform.position, targetCamPos, this.smoothing * Time.deltaTime);
        }
    }
}
