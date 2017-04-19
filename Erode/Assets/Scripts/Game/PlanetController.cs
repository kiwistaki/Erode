using UnityEngine;

namespace Assets.Scripts.Game
{
    class PlanetController : MonoBehaviour
    {
        void Update()
        {
            this.transform.Rotate(Vector3.up, 0.5f * Time.deltaTime);
        }
    }
}
