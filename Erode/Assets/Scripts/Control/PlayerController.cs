using UnityEngine;

namespace Assets.Scripts.Control
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject HammerRoot;
        public GameObject RepairRoot;
        public GameObject RepairVFX;
        public GameObject HitVfxPrefab;
        public HammerController HammerController = null;
        public RepairToolController RepairToolController = null;
        [Range(0.0f, 10.0f)]
        public float MovementSpeed = 5.0f;
        [Range(0.0f, 100.0f)]
        public float Gravity = 9f;
        [Range(0.0f, 1.0f)]
        public float HammerChargeMinimalHold = 0.18f;
        [Range(5.0f, 50.0f)]
        public float RotationSmoothing = 15.0f;
        [Range(0.1f, 2.5f)]
        public float KnockbackTime = 1f;
        [Range(1.0f,100.0f)]
        public float AsteroidKnockbackStrenght = 25.0f;
        [Range(0.0f, 15.0f)]
        public float AsteroidAirKnockbackStrenght = 4.0f;
        [Range(0.0f, 2.5f)]
        public float BlitzMinTime = 0.00f;
        [Range(0.0f, 10.0f)]
        public float BlitzMaxTime = 5.0f;
        [Range(1.0f, 100.0f)]
        public float BlitzSpeed = 7.0f;
        [Range(0.0f, 15.0f)]
        public float BlitzTurnSpeed = 1.0f;
        [Range(0.0f, 15.0f)]
        public float BlitzStunTimeAsteroid = 2.0f;
        [Range(0.0f, 2.5f)]
        public float SpinMinTime = 0.5f;
        [Range(0.1f, 10.0f)]
        public float SpinRotateSpeed = 1.0f;
        [Range(0.1f, 10.0f)]
        public float SpinMoveSpeed = 1.0f;
        [Range(0.0f, 30.0f)]
        public float SpinMoveSpeedDecay = 5.0f;
        [Range(0.0f, 1.0f)]
        public float RepairTurnSpeed = 0.2f;
        [Range(10f, 50f)]
        public float RepairSpeed = 20f;
        [Range(10f, 30f)]
        public float RepairDistance = 20f;
        public LayerMask RepairLayerMask;

        #region Enumeration
        public enum EquippedWeapons
        {
            Undefined = -1,
            None,
            Hammer,
            Repair
        }
        #endregion

        #region Events
        public delegate void StrikeAnimCompleteEventHandler();
        public event StrikeAnimCompleteEventHandler StrikeAnimCompleteEvent;
        public void OnStrikeAnimComplete()
        {
            if (this.StrikeAnimCompleteEvent != null)
            {
                this.StrikeAnimCompleteEvent();
            }
        }

        public delegate void AnimTransitionEventHandler();
        public event AnimTransitionEventHandler AnimTransitionEvent;
        public void OnAnimTransitionEvent()
        {
            if (this.AnimTransitionEvent != null)
            {
                this.AnimTransitionEvent();
            }
        }

        public delegate void HunterAttackEventHandler(float stunnedTime);
        public event HunterAttackEventHandler HunterAttackEvent;
        public void OnHunterAttackEvent(float stunnedTime)
        {
            if(this.HunterAttackEvent != null)
            {
                this.HunterAttackEvent(stunnedTime);
            }
        }

        public delegate void HunterCollisionEventHandler(GameObject hunter);
        public event HunterCollisionEventHandler HunterCollisionEvent;
        public void OnHunterCollisionEvent(GameObject hunter)
        {
            if (this.HunterCollisionEvent != null)
            {
                this.HunterCollisionEvent(hunter);
            }
        }

        public delegate void AsteroidCollisionEventHandler(GameObject arg);
        public event AsteroidCollisionEventHandler AsteroidCollisionEvent;
        public void OnAsteroidCollision(GameObject arg)
        {
            if (this.AsteroidCollisionEvent != null)
            {
                this.AsteroidCollisionEvent(arg);
            }
        }

        public delegate void CometCollisionEventHandler(GameObject arg);
        public event CometCollisionEventHandler CometCollisionEvent;
        public void OnCometCollision(GameObject arg)
        {
            if (this.CometCollisionEvent != null)
            {
                this.CometCollisionEvent(arg);
            }
        }
        #endregion

        public float SpeedModifier { get; set; }

        private CharacterController _playerCharacterController = null;
        public CharacterController PlayerCharacterController
        {
            get { return this._playerCharacterController; }
        }

        private Animator _playerAnimator = null;
        public Animator PlayerAnimator
        {
            get { return this._playerAnimator; }
        }

        private PlayerCharacterStateMachine _playerCharacterStateMachine = null;

        void Awake()
        {
            this.SpeedModifier = 1.0f;

            //Subscribe to collisions
            this.AsteroidCollisionEvent += this.DefaultCollision;
            this.CometCollisionEvent += this.DefaultCollision;
        }

        void Start()
        {
            this._playerCharacterStateMachine = new PlayerCharacterStateMachine(this);
            this._playerCharacterController = this.GetComponent<CharacterController>();
            this._playerAnimator = this.GetComponent<Animator>();

            this.PlayerAnimator.SetFloat("MoveSpeed", 0.0f);
            this.PlayerAnimator.SetFloat("ForwardVelocity", 0.0f);
            this.PlayerAnimator.SetFloat("RightVelocity", 0.0f);
        }

        //Update is called once per frame
        void Update()
        {
            //Gravity is important!
            this.ApplyGravity();

            this._playerCharacterStateMachine.UpdateStateMachine();
        }

        public void ResetPlayer()
        {
            this.gameObject.transform.position = new Vector3(0, 0, 0);
        }

        public void ProcessMovementRotationFreeInput(float moveMultiplier, float rotationMultiplier)
        {
            //Capture inputs
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            //Diagonal movement speed needs to be clamped.
            if (input.magnitude > 1.0f)
            {
                input.Normalize();
            }

            //Apply rotation
            if (input != Vector3.zero)
            {
                this.RotatePlayer(rotationMultiplier, input);
            }

            //Apply movement
            this.MovePlayer(input, this.MovementSpeed * moveMultiplier);
        }

        public void ProcessMovementRotationLockInput(float moveMultiplier, float rotationMultiplier)
        {
            //Capture inputs
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            //Diagonal movement speed needs to be clamped.
            if (input.magnitude > 1.0f)
            {
                input.Normalize();
            }

            //Apply rotation
            if (input != Vector3.zero)
            {
                this.RotatePlayer(rotationMultiplier, input);
            }

            //Apply movement
            this.MovePlayer(this.transform.forward, input.magnitude * this.MovementSpeed * moveMultiplier);
        }

        public void ProcessRotationInput(float rotationMultiplier)
        {
            //Capture inputs
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            //Apply rotation
            if (input != Vector3.zero)
            {
                this.RotatePlayer(rotationMultiplier,input);
            }
        }

        public void ApplyGravity()
        {
            //Apply movement
            this.PlayerCharacterController.Move(Vector3.down * this.Gravity * Utils.Utils.getRealDeltaTime());
        }

        public void EquipWeapons(EquippedWeapons equip)
        {
            switch (equip)
            {
                case EquippedWeapons.Undefined:
                    throw new UnityException("PlayerController::EquipWeapons: Usage of undefined enum");

                case EquippedWeapons.None:
                    this.HammerRoot.SetActive(false);
                    this.RepairRoot.SetActive(false);
                    this.RepairVFX.SetActive(false);
                    this.SetHammerType(HammerController.HammerType.Disabled);
                    this.SetRepairToolType(RepairToolController.RepairToolStatus.Disabled);
                    break;

                case EquippedWeapons.Hammer:
                    this.RepairRoot.SetActive(false);
                    this.HammerRoot.SetActive(true);
                    this.RepairVFX.SetActive(false);
                    //When a state wants to show the hammer, it must define itself the hammer type
                    this.SetHammerType(HammerController.HammerType.Undefined);
                    this.SetRepairToolType(RepairToolController.RepairToolStatus.Disabled);
                    break;

                case EquippedWeapons.Repair:
                    this.HammerRoot.SetActive(false);
                    this.RepairRoot.SetActive(true);
                    this.RepairVFX.SetActive(true);
                    this.SetHammerType(HammerController.HammerType.Disabled);
                    this.SetRepairToolType(RepairToolController.RepairToolStatus.Enabled);
                    break;

                default:
                    throw new UnityException("PlayerController::EquipWeapons: " + equip.ToString() + " IS NOT IMPLEMENTED");
            }
        }

        public void SetHammerType(HammerController.HammerType type)
        {
            this.HammerController.StrikeHammerType = type;
        }

        public void SetRepairToolType(RepairToolController.RepairToolStatus type)
        {
            this.RepairToolController.RepairingStatus = type;
        }

        public void DefaultCollision(GameObject asteroid)
        {
            this.PlayHitEffect(this.transform.position + (asteroid.transform.position - this.transform.position) * 0.5f);
            this.ChangeState(PlayerCharacterStateMachine.PlayerStates.Knockbacked, asteroid);
        }

        public void HitByHunterAttack(float stunnedTime)
        {
            this.PlayHitEffect(this.transform.position + 2 * this.transform.up);
            this.ChangeState(PlayerCharacterStateMachine.PlayerStates.Stunned, stunnedTime);
        }

        public void ChangeState(PlayerCharacterStateMachine.PlayerStates state, object args)
        {
            this._playerCharacterStateMachine.ChangeState(state, args);
        }

        public void ChangeState(PlayerCharacterStateMachine.PlayerStates state)
        {
            this._playerCharacterStateMachine.ChangeState(state, null);
        }

        public void RotatePlayer(float rotationMultiplier, Vector3 input)
        {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(input.normalized), this.RotationSmoothing * rotationMultiplier * Utils.Utils.getRealDeltaTime());
        }

        public void MovePlayer(Vector3 direction, float speed)
        {
            speed *= direction.magnitude;
            direction.Normalize();
            //Send inputs to animator
            var animatorSpeed = speed*this.SpeedModifier/this.MovementSpeed;
            this.PlayerAnimator.SetFloat("MoveSpeed", animatorSpeed);
            //Move character
            this.PlayerCharacterController.Move(direction * speed * this.SpeedModifier * Utils.Utils.getRealDeltaTime());
        }

        public void PlayHitEffect(Vector3 pos)
        {
            Instantiate(this.HitVfxPrefab, pos, this.transform.rotation);
        }
    }
}