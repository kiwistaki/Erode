using Assets.Scripts.HexGridGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Control
{
    class RepairingState : PlayerState
    {
        private Vector3 _lastRepairDirection;
        private float _angleRotation = 0;
        private int _angleVariation = 1; // 1 forward, -1 backward
        public RepairingState(PlayerController player) 
            : base(player, null)
        {
        }

        public override void Enter()
        {
            this._playerController.PlayerAnimator.SetTrigger("RepairStart");
            this._playerController.EquipWeapons(PlayerController.EquippedWeapons.Repair);
            this._lastRepairDirection = Quaternion.AngleAxis(_angleRotation, _playerController.transform.right) * -_playerController.transform.up;

            //Register to hunter attack event
            this._playerController.HunterAttackEvent += this._playerController.HitByHunterAttack;
        }

        public override void Exit()
        {
            this._playerController.PlayerAnimator.SetTrigger("RepairEnd");
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.Repairing;
        }

        public override void OnStateUpdate()
        {
            // Check right trigger
            if(Input.GetAxisRaw("Fire4") <= 0.0f)
            {
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.IdleRun);
                return;
            }

            CheckForRotation();
            UpdateRepairDirection();
            DoRepair();          
        }

        private void CheckForRotation()
        {
            Quaternion temp = this._playerController.transform.rotation;
            this._playerController.ProcessRotationInput(this._playerController.RepairTurnSpeed);
            if (temp != this._playerController.transform.rotation)
            {
                _angleRotation = 0;
                _lastRepairDirection = Quaternion.AngleAxis(_angleRotation, _playerController.transform.right) * -_playerController.transform.up;
            }
        }

        private void UpdateRepairDirection()
        {
            if (_angleRotation > -_playerController.RepairDistance)
            {
                float deltaAngle = -_playerController.RepairSpeed * Utils.Utils.getRealDeltaTime() * _angleVariation;
                _lastRepairDirection = Quaternion.AngleAxis(deltaAngle, _playerController.transform.right) * _lastRepairDirection;
                _angleRotation += deltaAngle;
            }
            Debug.DrawRay(_playerController.transform.position + new Vector3(0, 15f, 0), _lastRepairDirection * 50, Color.red);
        }

        private void DoRepair()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(_playerController.transform.position + new Vector3(0, 15f, 0), _lastRepairDirection), out hitInfo, 100, _playerController.RepairLayerMask))
            {
                Tile hittedTile = hitInfo.collider.gameObject.GetComponent<Tile>();
                _angleVariation = 1;
                // Repair neighbors
                if (hittedTile != null)
                {
                    hittedTile.Hp = hittedTile._maxHp;
                    foreach (Tile t in hittedTile.Neighbours)
                    {
                        t.Hp = t._maxHp;
                    }
                }
            }
            else
            {
                if (_angleVariation == 1)
                    _angleVariation = -1;
            }
        }
    }
}
