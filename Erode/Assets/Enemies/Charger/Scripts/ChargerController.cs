using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Assets.Scripts.Control;
using UnityEngine;
using Assets.Enemies;

public class ChargerController : MonoBehaviour
{

    // Hunter Params
    public Transform Target;
    public float MoveSpeed = 10.0f;
    public float Gravity = 9f;
    public float RotationSmoothing = 3.0f;
    public float DetectionRange = 4f;
    public float AsteroidKnockbackTime = 0.50f;
    public float AsteroidKnockbackStrenght = 6.0f;
    public float AsteroidAirKnockbackStrenght = 1.0f;
    Vector3 ChargeDestination = new Vector3(0,0,0);
    public int HitPoint
    {
        get { return _hitPoint; }
        set
        {
            if (!_isLosingHp)
            {
                if (_healthBarController != null)
                {
                    _healthBarController.ShowHealthBar();
                    _healthBarController.ChangeHealth(value, MaxHitPoint);
                }
                _hitPoint = value;
                _isLosingHp = true;
                StartCoroutine(WaitAfterLosingHp());
            }
        }
    }

    private IEnumerator WaitAfterLosingHp()
    {
        yield return new WaitForSeconds(0.5f);
        _isLosingHp = false;
    }

    private int _hitPoint = 3;
    public int MaxHitPoint = 3;

    private Vector3 PlayerPosCharge = new Vector3(0,0,0);
    
    private ScoreManager _scoreManager;
    private HealthBarController _healthBarController;


    private Transform myTransform;

    private Animator _chargerAnimator = null;
    public Animator ChargerAnimator
    {
        get { return this._chargerAnimator; }
    }

    private CharacterController _chargerCharacterController = null;
    public CharacterController ChargerCharacterController
    {
        get { return this._chargerCharacterController; }
    }

    private ChargerStateMachine _chargerStateMachine = null;
    public ChargerStateMachine ChargerStateMachine
    {
        get { return this._chargerStateMachine; }
    }

    private bool isDead = false;
    private bool _isLosingHp = false;

    
    #region Events
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

    public delegate void AttackAnimCompleteEventHandler();
    public event AttackAnimCompleteEventHandler AttackAnimCompleteEvent;
    public void AttackAnimComplete()
    {
        if (this.AttackAnimCompleteEvent != null)
        {
            this.AttackAnimCompleteEvent();
        }
    }

    public delegate void DieAnimCompleteEventHandler();
    public event DieAnimCompleteEventHandler DieAnimCompleteEvent;
    public void DieAnimComplete()
    {
        if (this.DieAnimCompleteEvent != null)
        {
            this.DieAnimCompleteEvent();
        }
    }

    public delegate void ShoutAnimCompleteEventHandler(GameObject arg);
    public event ShoutAnimCompleteEventHandler ShoutAnimCompleteEvent;
    public void ShoutAnimComplete(GameObject arg)
    {
        if (this.ShoutAnimCompleteEvent != null)
        {
            this.ShoutAnimCompleteEvent(arg);
        }
    }
    #endregion
    
    void Awake()
    {
        this.AsteroidCollisionEvent += this.DefaultCollision;
        this.CometCollisionEvent += this.DefaultCollision;
        this.DieAnimCompleteEvent += this.DefaultDieAnimComplete;
        this.ShoutAnimCompleteEvent += this.DefaultShoutAnimComplete;

        this._scoreManager = GameObject.Find("MainCamera").GetComponent<ScoreManager>();
        this._healthBarController = this.GetComponent<HealthBarController>();
    }

    // Use this for initialization
    void Start()
    {
        myTransform = this.transform;

        this.Target = GameObject.FindGameObjectWithTag("Player").transform;
        this._chargerAnimator = this.GetComponent<Animator>();
        this._chargerStateMachine = new ChargerStateMachine(this);
        this._chargerCharacterController = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        this._chargerStateMachine.UpdateStateMachine();
        if (HitPoint <= 0 && !isDead)
        {
            isDead = true;
            _scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.Charger, this.transform.parent.name);
            _scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.Charger, this.transform.position);
            this.ChangeState(ChargerStateMachine.ChargerState.Die);
        }
        this.ApplyGravity();
    }

    public bool WillCharge()
    {
        float distance = Vector3.Distance(myTransform.position, Target.position);
        return distance < DetectionRange;
    }
    public void turnTowardsTarget()
    {
        var lookat = new Vector3(Target.position.x - myTransform.position.x, 0, Target.position.z - myTransform.position.z);
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(lookat), RotationSmoothing * Time.deltaTime);
    }

    public void chargeDestination() 

    {
        ChargeDestination = Target.position;
        Vector3 dist = (Target.position - myTransform.position);
        Vector3 direction = dist.normalized;

        ChargeDestination = Target.position + direction * 3;
    }

    public bool hasReachedOldPlayerPos() 
    {
        bool reached = false;
        if (Mathf.Abs(myTransform.position.x - ChargeDestination.x) <= 0.3 && Mathf.Abs(myTransform.position.z - ChargeDestination.z) <= 0.3) 
        {
            reached = true;
        }
        return reached;
    }

    public void chargePlayer() 
    {
        float step = MoveSpeed * Time.deltaTime;       
        
        if (this._chargerCharacterController.enabled)
        {
            this._chargerCharacterController.transform.position = Vector3.MoveTowards(myTransform.position, ChargeDestination, step);
        }
    }

    public void ApplyGravity()
    {
        if (this._chargerCharacterController.enabled)
        {
            this._chargerCharacterController.Move(Vector3.down * this.Gravity * Time.deltaTime);
        }
    }

    public void ProcessMovement()
    {
        var lookat = new Vector3(Target.position.x - myTransform.position.x, 0, Target.position.z - myTransform.position.z);
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(lookat), RotationSmoothing * Time.deltaTime);
        myTransform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (ChargerStateMachine.returnEnumValue() == (int)ChargerStateMachine.ChargerState.Attack ||
                ChargerStateMachine.returnEnumValue() == (int)ChargerStateMachine.ChargerState.Run) 
            {
                other.GetComponent<PlayerController>().OnAsteroidCollision(this.gameObject);
            }
            
        }
        else if (other.tag == "Hammer")
        {
                other.GetComponent<HammerController>().OnChargerCollision(this.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Target":
            case "SpinReflect":
            case "Boundary":
                //do nothing
                break;

            default:
                //Push the Charger around whatever is triggering it
                this.transform.position += (this.transform.position - other.transform.position) * Time.deltaTime;
                break;
        }
    }

    public void DefaultCollision(GameObject arg)
    {
        this._chargerStateMachine.ChangeState(ChargerStateMachine.ChargerState.Knockbacked , arg);
    }

    public void DefaultDieAnimComplete()
    {
        Destroy(this.gameObject);
    }

    public void DefaultShoutAnimComplete(GameObject arg) 
    {
        this._chargerStateMachine.ChangeState(ChargerStateMachine.ChargerState.Run, arg);
    }

    public void ChangeState(ChargerStateMachine.ChargerState state, object args)
    {
        this._chargerStateMachine.ChangeState(state, args);
    }
    public void ChangeState(ChargerStateMachine.ChargerState state)
    {
        this._chargerStateMachine.ChangeState(state, null);
    }
}
