using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using UnityEngine;
using Assets.Enemies;

public class ShooterController : MonoBehaviour
{

    // Shooter var
    public Transform    Target;
    public GameObject   Bolt;
    public GameObject   ShooterGun;
    public float        TimeBetweenBolt = 1.5f;
    public float        MinRangeFollow = 6.0f;
    public float        MaxRangeFollow = 3.0f;
    public float        MinRangeAttack = 6.0f;
    public float        MoveSpeed = 3.0f;
    public float        RotationSmoothing = 5.0f;
    public float        CometKnockbackTime = 0.75f;
    public float        CometKnockbackStrenght = 10.0f;
    public float        CometAirKnockbackStrenght = 0.0f;
    public float        Gravity = 9.0f;
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

    private int _hitPoint = 2;
    public int MaxHitPoint = 2;

    private bool        _deadManWalking = false;
    private bool _isLosingHp = false;
    private ScoreManager _scoreManager;
    private HealthBarController _healthBarController;

    private Animator    _shooterAnimator = null;
    public Animator     ShooterAnimator
    {
        get { return this._shooterAnimator; }
    }

    private CharacterController _shooterCharacterController = null;
    public CharacterController ShooterCharacterController
    {
        get { return this._shooterCharacterController; }
    }

    private ShooterCharacterStateMachine _shooterCharacterStateMachine = null;

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
        if(this.CometCollisionEvent != null)
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
    #endregion


    public void Awake()
    {
        this.AsteroidCollisionEvent += this.DefaultAsteroidCollision;
        this.CometCollisionEvent += this.DefaultAsteroidCollision;
        this.DieAnimCompleteEvent += this.DefaultDieAnimComplete;
        this.AttackAnimCompleteEvent += this.DefaultShootAnimComplete;

        this._scoreManager = GameObject.Find("MainCamera").GetComponent<ScoreManager>();
        this._healthBarController = this.GetComponent<HealthBarController>();
    }

    public void Start()
    {
        this.Target = GameObject.FindGameObjectWithTag("Player").transform;
        this._shooterAnimator = this.GetComponent<Animator>();
        this._shooterCharacterStateMachine = new ShooterCharacterStateMachine(this);
        this._shooterCharacterController = this.GetComponent<CharacterController>();
    }

    public void Update()
    {
        this._shooterCharacterStateMachine.UpdateStateMachine();

        if (HitPoint <= 0 && !this._deadManWalking)
        {
            this._deadManWalking = true;
            _scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.Shooter, this.transform.parent.name);
            _scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.Shooter, this.transform.position);
            this.ChangeState(ShooterCharacterStateMachine.ShooterState.Die);
        }
    }

    public void ProcessMovement()
    {
        var lookat = new Vector3(Target.position.x - this.transform.position.x, 0, Target.position.z - this.transform.position.z);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(lookat), RotationSmoothing * Time.deltaTime);
        var movement = this.transform.forward * MoveSpeed * Time.deltaTime;
        movement.y = 0;
        this.transform.position += movement;
    }

    public void ProcessBackoffMovement()
    {
        var lookat =-1* (new Vector3(Target.position.x - this.transform.position.x, 0, Target.position.z - this.transform.position.z));
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(lookat), RotationSmoothing * Time.deltaTime);
        var movement = this.transform.forward * MoveSpeed * 1.1f * Time.deltaTime;
        movement.y = 0;
        this.transform.position += movement;
    }

    public void ProcessOrbitMovement()
    {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                Quaternion.LookRotation(this.Target.transform.position - this.transform.position), Time.deltaTime * 5);
        var movement = this.transform.right * this.MoveSpeed * Time.deltaTime;
        movement.y = 0;
        this.transform.position += movement;
    }

    public void Shoot()
    {
        Vector3 gunPos = this.ShooterGun.transform.position;
        var b = Instantiate(this.Bolt, gunPos, Quaternion.identity);
        Debug.Log(b.transform.position.ToString());
    }

    public bool WillFollow()
    {
        return Vector3.Distance(this.transform.position, Target.position) > MinRangeFollow;
    }

    public bool WillBackoff()
    {
        return Vector3.Distance(this.transform.position, Target.position) < MaxRangeFollow;
    }

    public bool ShouldContinueBackoff()
    {
        return Vector3.Distance(this.transform.position, Target.position) < MaxRangeFollow * 1.25f;
    }

    public bool IsWithinAttackRange()
    {
        return Vector3.Distance(this.transform.position, Target.position) < MinRangeAttack;
    }

    public bool ShouldContinueFollowing()
    {
        return Vector3.Distance(this.transform.position, Target.position) < MinRangeAttack * 0.75f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().OnShooterCollisionEvent(this.gameObject);
        }
        else if (other.tag == "Hammer")
        {
            other.GetComponent<HammerController>().OnShooterCollision(this.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Target":
                //do nothing
                break;

            case "Boundary":
                //do nothing
                break;

            default:
                //Push the Shooter around whatever is triggering it
                if(this != null && other != null)
                    this.transform.position += (this.transform.position - other.transform.position) * Time.deltaTime;
                break;
        }
    }

    public void DefaultAsteroidCollision(GameObject arg)
    {
        this._shooterCharacterStateMachine.ChangeState(ShooterCharacterStateMachine.ShooterState.Knockbacked, arg);
    }

    public void DefaultDieAnimComplete()
    {

        Destroy(gameObject);
    }

    public void DefaultShootAnimComplete()
    {
        this._shooterCharacterStateMachine.ChangeState(ShooterCharacterStateMachine.ShooterState.Idle, null);
    }

    public void ChangeState(ShooterCharacterStateMachine.ShooterState state, object args)
    {
        this._shooterCharacterStateMachine.ChangeState(state, args);
    }
    public void ChangeState(ShooterCharacterStateMachine.ShooterState state)
    {
        this._shooterCharacterStateMachine.ChangeState(state, null);
    }
}
