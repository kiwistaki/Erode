using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Assets.Scripts.Control;
using UnityEngine;
using Assets.Enemies;

public class HunterController : MonoBehaviour
{

    // Hunter Params
    public Transform    Target;
    public float        MinRangeFollow = 8.0f;
    public float        MinRangeAttack = 2.0f;
    public float        MoveSpeed = 5.0f;
    public float        RotationSmoothing = 5.0f;
    public float        Gravity = 9f;
    public float        AsteroidKnockbackTime = 0.75f;
    public float        AsteroidKnockbackStrenght = 10.0f;
    public float        AsteroidAirKnockbackStrenght = 0.0f;
    public float        PlayerStunnedTime = 0.5f;
    public int          HitPoint
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
    public int  MaxHitPoint = 3;

    private bool deadManWalking = false;
    private bool _isLosingHp = false;
    private Transform   myTransform;
    private ScoreManager _scoreManager;
    private HealthBarController _healthBarController;

    private Animator _hunterAnimator = null;
    public Animator HunterAnimator
    {
        get { return this._hunterAnimator; }
    }

    private CharacterController _hunterCharacterController = null;
    public CharacterController HunterCharacterController
    {
        get { return this._hunterCharacterController; }
    }

    private HunterCharacterStateMachine _hunterCharacterStateMachine = null;

    #region Events
    public delegate void AsteroidCollisionEventHandler(GameObject arg);
    public event AsteroidCollisionEventHandler AsteroidCollisionEvent;
    public void OnAsteroidCollision(GameObject arg)
    {
        if(this.AsteroidCollisionEvent != null)
        {
            this.AsteroidCollisionEvent(arg);
        }
    }

    public delegate void AttackAnimCompleteEventHandler();
    public event AttackAnimCompleteEventHandler AttackAnimCompleteEvent;
    public void AttackAnimComplete()
    {
        if(this.AttackAnimCompleteEvent != null)
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
    #endregion

    void Awake()
    {
        this.AsteroidCollisionEvent += this.DefaultAsteroidCollision;
        this.DieAnimCompleteEvent += this.DefaultDieAnimComplete;

        this._scoreManager = GameObject.Find("MainCamera").GetComponent<ScoreManager>();
    }

    // Use this for initialization
    void Start ()
    {
        myTransform = this.transform;
        this.Target = GameObject.FindGameObjectWithTag("Player").transform;
        this._hunterAnimator = this.GetComponent<Animator>();
        this._hunterCharacterStateMachine = new HunterCharacterStateMachine(this);
        this._hunterCharacterController = this.GetComponent<CharacterController>();
        this._healthBarController = this.GetComponent<HealthBarController>();
    }

    // Update is called once per frame
    void Update ()
    {
        this._hunterCharacterStateMachine.UpdateStateMachine();
        if(HitPoint <= 0 && !deadManWalking)
        {
            deadManWalking = true;
            _scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.Hunter, this.transform.parent.name);
            _scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.Hunter, this.transform.position);
            this.ChangeState(HunterCharacterStateMachine.HunterState.Die);
        }
        this.ApplyGravity();
    }

    public bool WillFollow()
    {
        return Vector3.Distance(myTransform.position, Target.position) < MinRangeFollow;
    }

    public bool IsWithinAttackRange()
    {
        return Vector3.Distance(myTransform.position, Target.position) < MinRangeAttack;
    }

    public void ApplyGravity()
    {
        if (this._hunterCharacterController.enabled)
        {
            this._hunterCharacterController.Move(Vector3.down*this.Gravity*Time.deltaTime);
        }
    }

    public void ProcessMovement()
    {
        var lookat = new Vector3(Target.position.x - myTransform.position.x, 0, Target.position.z - myTransform.position.z);
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(lookat), RotationSmoothing * Time.deltaTime);
        myTransform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
    }

    public void Attack()
    {
        if(this.IsWithinAttackRange())
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerController>().OnHunterAttackEvent(this.PlayerStunnedTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().OnHunterCollisionEvent(this.gameObject);
        }
        else if (other.tag == "Hammer")
        {
            other.GetComponent<HammerController>().OnHunterCollision(this.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Target":
            case "Boundary":
            case "SpinReflect":
                //do nothing
                break;

            default:
                //Push the Hunter around whatever is triggering it
                this.transform.position += (this.transform.position - other.transform.position) * Time.deltaTime;
                break;
        }
    }

    public void DefaultAsteroidCollision(GameObject arg)
    {
        this._hunterCharacterStateMachine.ChangeState(HunterCharacterStateMachine.HunterState.Knockbacked, arg);
    }

    public void DefaultDieAnimComplete()
    {
        Destroy(gameObject);
    }

    public void ChangeState(HunterCharacterStateMachine.HunterState state, object args)
    {
        this._hunterCharacterStateMachine.ChangeState(state, args);
    }
    public void ChangeState(HunterCharacterStateMachine.HunterState state)
    {
        this._hunterCharacterStateMachine.ChangeState(state, null);
    }
}
