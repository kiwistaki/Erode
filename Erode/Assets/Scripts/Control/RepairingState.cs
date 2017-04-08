using Assets.Scripts.HexGridGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Control
{
    class RepairingState : PlayerState
    {
        private Vector3 _lastRepairDirection;
        private float _angleRotation = 0;
        private int _angleVariation = 1; // 1 forward, -1 backward

        private int[] _immobileAngles = { -25, -45, -60 };
        private int _immobileIt = 0;

        private float _boxTimer = 0.0f;

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
            this._playerController.ShooterAttackEvent += this._playerController.HitByShooterAttack;
        }

        public override void Exit()
        {
            this._playerController.PlayerAnimator.SetTrigger("RepairEnd");
            this._playerController.HunterAttackEvent -= this._playerController.HitByHunterAttack;
            this._playerController.ShooterAttackEvent -= this._playerController.HitByShooterAttack;
        }

        public override PlayerCharacterStateMachine.PlayerStates GetStateType()
        {
            return PlayerCharacterStateMachine.PlayerStates.Repairing;
        }

        public override void OnStateUpdate()
        {
            if (this._playerController.BoxRepair)
            {
                //THIS IS THE BOX REPAIR
                this.MovingBoxRepair();
                this._playerController.ProcessMovementRotationFreeInput(this._playerController.RunningRepairMoveSpeed, this._playerController.RunningRepairTurnSpeed);
            }
            else if (this._playerController.RunningRepair)
            {
                //THIS IS THE MOBILE REPAIR
                this.MobileRepair();
                this._playerController.ProcessMovementRotationFreeInput(this._playerController.RunningRepairMoveSpeed, this._playerController.RunningRepairTurnSpeed);
            }
            else
            {
                //THIS IS THE IMMOBILE REPAIR
                CheckForRotation();
                UpdateRepairDirection();
                DoRepair();
            }

            // Check right trigger
            if (Input.GetAxisRaw("Fire4") <= 0.0f || this._playerController.AmmoCount <= 0)
            {
                this._playerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.IdleRun);
                return;
            }
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
                //Repair neighbors
                if (hittedTile != null)
                {
                    hittedTile.RepairFromGun(this._playerController.RepairToolController.transform.position);
                    foreach (Tile t in hittedTile.Neighbours)
                    {
                        t.RepairFromGun(this._playerController.RepairToolController.transform.position);
                    }
                }
            }
            else
            {
                if (_angleVariation == 1)
                    _angleVariation = -1;
            }
        }

        private void MobileRepair()
        {
            RaycastHit hitInfo;
            if (this._immobileIt >= this._immobileAngles.Length)
            {
                this._immobileIt = 0;
            }
            var dir = Quaternion.AngleAxis(this._immobileAngles[this._immobileIt++], this._playerController.transform.right) * -this._playerController.transform.up;
            var ra = new Ray(this._playerController.transform.position, dir);

            var success = Physics.Raycast(ra, out hitInfo, 100, this._playerController.RepairLayerMask);

            if (success)
            {
                Tile hittedTile = hitInfo.collider.gameObject.GetComponent<Tile>();

                //Repair neighbors
                if (hittedTile != null)
                {
                    hittedTile.RepairFromGun(this._playerController.RepairToolController.transform.position);
                    foreach (Tile t in hittedTile.Neighbours)
                    {
                        t.RepairFromGun(this._playerController.RepairToolController.transform.position);
                    }
                }
            }
        }

        private void MovingBoxRepair()
        {
            if ((this._boxTimer += Utils.Utils.getRealDeltaTime()) >= this._playerController.BoxRepairTimer && this._playerController.AmmoCount > 0)
            {
                var tiles = this._playerController.RepairBoxController.CollidingTiles.Values;
                float min = 0.0f;
                Tile minTile = null;
                foreach (var tile in tiles)
                {
                    if(tile.Hp != 0)
                        continue;

                    var val = (tile.transform.position - this._playerController.RepairRoot.transform.position).magnitude;
                    if (val < min || minTile == null)
                    {
                        min = val;
                        minTile = tile;
                    }
                }
                if (minTile != null)
                {
                    minTile.RepairFromGun(this._playerController.RepairToolController.transform.position);
                    this._boxTimer -= this._playerController.BoxRepairTimer;
                    this._playerController.AmmoCount -= 1;
                    this._playerController.RepairBoxController.CollidingTiles.Remove(minTile.GetHashCode());
                }
            }
        }
    }
}