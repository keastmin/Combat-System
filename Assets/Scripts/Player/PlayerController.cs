using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(InputController), typeof(PlayerMover))]
public class PlayerController : MonoBehaviour
{
    [Header("Debug")]
    public TMP_Text StateText;
    
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _jogSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;

    [Header("Dodge")]
    [SerializeField] private float _dodgeSpeed = 8f;
    [SerializeField] private float _dodgeTime = 1f;

    // state context
    [SerializeField] private StateContext _stateContext;

    // move
    private int _moveLevel = 1; // 0: walk, 1: jog, 2: run
    
    // velocity
    private float _targetSpeed;
    private float _currentSpeed;

    // rotation
    private Transform _cameraTransform;
    private float _rotationSpeed = 10f;

    // properties
    public float WalkSpeed => _walkSpeed;
    public float JogSpeed => _jogSpeed;
    public float RunSpeed => _runSpeed;
    public float DodgeSpeed => _dodgeSpeed;
    public float DodgeTime => _dodgeTime;
    public float TargetSpeed { get => _targetSpeed; set => _targetSpeed = value; }
    public float CurrentSpeed { get => _currentSpeed; set => _currentSpeed = value; }
    public int MoveLevel { get => _moveLevel; set => _moveLevel = value; }
    public Vector3 Forward { get => transform.forward; }
    public PlayerStateMachine StateMachine => _stateMachine;

    private InputController _inputController;
    private PlayerMover _playerMover;
    private Animator _playerAnimator;
    private PlayerStateMachine _stateMachine;


    public InputController InputController => _inputController;
    public PlayerMover PlayerMover => _playerMover;
    public Animator PlayerAnimator => _playerAnimator;

    private void Awake()
    {
        TryGetComponent(out _inputController);
        TryGetComponent(out _playerMover);
        TryGetComponent(out _playerAnimator);

        _cameraTransform = Camera.main.transform;

        _stateMachine = new PlayerStateMachine(this);
    }

    void Start()
    {
        _stateMachine.InitState(_stateMachine.idleState);
    }

    void Update()
    {
        StateMachine.Execute();
    }

    private void FixedUpdate()
    {
        StateMachine.FixedExecute();
        PlayerAnimator.SetFloat("Speed", CurrentSpeed);
        PlayerAnimator.SetBool("IsGround", PlayerMover.IsGround);
    }

    #region Rotation

    public void RotationBasedCamera(float delta)
    {
        if (InputController.MoveInput.magnitude > 0)
        {
            Vector3 direction = CalculateDirection();

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta * _rotationSpeed);
        }
    }

    public Vector3 CalculateDirection()
    {
        Vector3 direction = Vector3.zero;

        if(_cameraTransform != null)
        {
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;
            forward.y = right.y = 0;

            direction = forward * InputController.MoveInput.y + right * InputController.MoveInput.x;
        }

        return direction;
    }

    #endregion
    
    #region Movement

    public void LerpSpeed(float delta)
    {
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, delta * 8f);
    }

    #endregion
}
