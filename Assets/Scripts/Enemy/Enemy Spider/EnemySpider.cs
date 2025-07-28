using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpider : NormalEnemy
{
    [Header("Detect Player")]
    [SerializeField] private float _detectRadius = 5f;
    [SerializeField] private LayerMask _playerLayerMask;

    [Header("Idle State")]
    [SerializeField] private float _minIdlePlayTime = 3f;
    [SerializeField] private float _maxIdlePlayTime = 8f;

    [Header("Patrol State")]
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _targetPointThreshold = 0.2f;

    [Header("Chase State")]
    [SerializeField] private float _chaseSpeed = 4f;

    [Header("Damaged State")]
    [SerializeField] private float _damagedCooldown = 0.3f;
    [SerializeField] private float _damagedTime = 0.7f;
    [SerializeField] private float _knockbackForce = 3f;

    #region Properties

    // Idle State Properties
    public float MinIdlePlayTime => _minIdlePlayTime;
    public float MaxIdlePlayTime => _maxIdlePlayTime;

    // Patrol State Properties
    public float PatrolSpeed => _patrolSpeed;
    public float TargetPointThreshold => _targetPointThreshold;

    // Chase State Properties
    public float ChaseSpeed => _chaseSpeed;

    // Damaged State Properties
    public float DamagedCooldown => _damagedCooldown;
    public float DamagedTime => _damagedTime;
    public float KnockbackForce => _knockbackForce;

    #endregion

    // Target
    private bool _isTargetDetected = false;
    private Transform _targetTransform;
    public bool IsTargetDetected => _isTargetDetected;
    public Transform TargetTransform => _targetTransform;

    // Rigidbody
    private Rigidbody _rigidbody;
    public Rigidbody SpiderRigidbody => _rigidbody;

    // Animator
    private Animator _animator;
    public Animator SpiderAnimator => _animator;

    // State Machine
    private SpiderStateMachine _stateMachine;
    public SpiderStateMachine StateMachine => _stateMachine;

    // Damaged
    private Vector3 _hitPoint = Vector3.zero;
    public Vector3 HitPoint => _hitPoint;
    private bool _isDamaged = false;
    public bool IsDamaged => _isDamaged;

    private void Awake()
    {
        InitComponents();
    }

    private void Start()
    {
        InitStateMachine();
    }

    private void Update()
    {
        DetectTarget();

        _stateMachine.Execute();
    }

    private void InitComponents()
    {
        TryGetComponent(out _rigidbody);
        TryGetComponent(out _animator);
    }

    private void InitStateMachine()
    {
        _stateMachine = new SpiderStateMachine(this);
        _stateMachine.InitState(_stateMachine.IdleState);
    }

    private void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            _detectRadius,
            _playerLayerMask
        );

        if (colliders.Length > 0)
        {
            _isTargetDetected = true;
            _targetTransform = colliders[0].transform;
        }
        else
        {
            _isTargetDetected = false;
            _targetTransform = null;
        }
    }

    public override void TakeDamage(int damage, Vector3 hitPoint)
    {
        base.TakeDamage(damage, hitPoint);
        _isDamaged = true;
        _hitPoint = hitPoint;
    }

    public void ClearDamage()
    {
        _isDamaged = false;
        _hitPoint = Vector3.zero;
    }
}
