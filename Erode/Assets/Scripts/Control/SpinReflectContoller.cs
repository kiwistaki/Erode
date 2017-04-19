using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class SpinReflectContoller : MonoBehaviour
    {
        public GameObject _playerController;

        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Bolt")
            {
                BoltController bolt = col.gameObject.GetComponent<BoltController>();
                if (bolt != null)
                {
                    bolt.ReverseDirection();
                }
            }
        }
    }
}
