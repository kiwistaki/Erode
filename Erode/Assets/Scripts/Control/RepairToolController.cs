using UnityEngine;

namespace Assets.Scripts.Control
{
    public class RepairToolController : MonoBehaviour
    {
        public enum RepairToolStatus
        {
            Undefined = -1,
            Enabled,
            Disabled
        }

        public PlayerController PlayerController;

        public RepairToolStatus RepairingStatus { get; set; }

        void Awake()
        {
            this.RepairingStatus = RepairToolStatus.Undefined;
        }
    }
}
