using System;
using UnityEngine;

[RequireComponent(typeof(InputController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("평행 움직임")]
    [SerializeField] private float _speedLerpTime = 8f; // 속도 보간 시간
    [SerializeField] private float _walkSpeed = 2f; // 걷기 속도
    [SerializeField] private float _jogSpeed = 5f; // 조깅 속도
    [SerializeField] private float _runSpeed = 8f; // 달리기 속도
    [SerializeField] private float _fastRunSpeed = 9f; // 빠른 달리기 속도
    [SerializeField] private float _fastRunStartTime = 3f; // 빠른 달리기 시작 시간

    [Header("회전")]
    [SerializeField] private float _rotationSpeed = 10f; // 회전 속도

    [Header("회피")]
    [SerializeField] private float _dodgeSpeed = 8f; // 회피 속도
    [SerializeField] private float _dodgeTime = 0.65f; // 회피 시간

    [Header("점프")]    
    [SerializeField] private float _jumpSpeed = 5f; // 점프 속도

    [Header("턴")]
    [SerializeField] private float _turnSpeed = 20f; // 턴 속도
    [SerializeField] private float _canTurnTime = 0.1f; // 달리기가 끝나고 턴이 가능한 시간


    // 필드 프로퍼티
    public float WalkSpeed => _walkSpeed; // 걷기 속도
    public float JogSpeed => _jogSpeed; // 조깅 속도
    public float RunSpeed => _runSpeed; // 달리기 속도
    public float FastRunSpeed => _fastRunSpeed; // 빠른 달리기 속도
    public float FastRunStartTime => _fastRunStartTime; // 빠른 달리기 시작 시간
    public float TurnSpeed => _turnSpeed; // 턴 속도
    public float DodgeSpeed => _dodgeSpeed; // 회피 속도
    public float DodgeTime => _dodgeTime; // 회피 시간
    public float RotationSpeed => _rotationSpeed; // 회전 속도

    // 속도
    private Vector3 _lastInputDirection; // 마지막 이동 입력 방향
    private float _currentSpeed; // 현재 속도
    private float _targetSpeed; // 목표 속도
    public float CurrentSpeed => _currentSpeed; // 현재 속도 프로퍼티

    // 상태 체크
    private float _runEndTime = 0f; // 달리기가 끝나고 지난 시간
    private bool _canTurn = false; // 턴 가능 여부
    private bool _isTurn = false; // 턴 트리거
    public bool CanTurn { get => _canTurn; set => _canTurn = value; } // 달리기 종료 후 턴 가능 여부 프로퍼티
    public bool IsTurn { get => _isTurn; set => _isTurn = value; } // 턴 트리거 프로퍼티

    // 카메라
    private Camera _mainCamera;

    // 상태 머신
    private PlayerStateMachine _stateMachine;
    public PlayerStateMachine StateMachine => _stateMachine;

    // 입력 컨트롤러
    private InputController _inputC;
    public InputController InputC => _inputC;

    // 애니메이터
    private Animator _anim;
    public Animator Anim => _anim;

    // 플레이어 무버
    private PlayerMover _mover;
    public PlayerMover Mover => _mover;
    
    private void Awake()
    {
        InitComponent();
        InitCamera();
    }

    private void Start()
    {
        InitStateMachine();
    }

    private bool _leaveGround = false;

    private void Update()
    {
        Debug.Log(_inputC.MoveInput);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLeaveGround();
        }

        LerpCurrentSpeed(_targetSpeed, _currentSpeed, _speedLerpTime); // 속도 세팅
        TurnChecker(); // 턴 체크
        _stateMachine?.Execute();
    }

    private void ToggleLeaveGround()
    {
        _leaveGround = !_leaveGround;
        Debug.Log("작동");

        if (_leaveGround)
        {
            _mover.LeaveGround();
        }
        else
        {
            _mover.EndLeaveGround();
        }
    }

    private void FixedUpdate()
    {
        _stateMachine?.FixedExecute();
    }

    private void OnAnimatorMove()
    {
        _stateMachine?.AnimatorMove();
    }


    #region 코어 함수

    #region 움직임

    // 현재 스피드와 타겟 스피드를 보간하여 속도를 조정
    private void LerpCurrentSpeed(float targetSpeed, float currentSpeed, float lerpTime)
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, lerpTime * Time.deltaTime);
        _currentSpeed = currentSpeed;
    }

    // 마지막 달리기로부터 턴이 가능한지 체크하는 함수
    private void TurnChecker()
    {
        if (_canTurn || _currentSpeed >= _jogSpeed + 0.1f)
        {
            Vector3 cameraForward = _mainCamera.transform.forward;
            Vector3 cameraRight = _mainCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 inputDir = cameraForward * _inputC.MoveInput.z + cameraRight * _inputC.MoveInput.x;
            inputDir.y = 0f;
            inputDir.Normalize();

            Vector3 forward = transform.forward;
            forward.y = 0f;

            float angle = Vector3.Angle(forward, inputDir);

            // 조건: 충분히 반대 방향이고, 입력이 존재해야 함
            if (angle > 120f && inputDir.sqrMagnitude > 0.1f)
            {
                _isTurn = true;
                _canTurn = false;
                _runEndTime = 0f;
                return;
            }

            // 유예 시간 감소
            _runEndTime += Time.deltaTime;
            if (_runEndTime > _canTurnTime)
            {
                _canTurn = false;
                _runEndTime = 0f; // 유예 시간 초기화
            }
        }
    }

    #endregion

    #endregion



    #region 상태 머신 사용 함수

    #region 움직임

    // 타겟 스피드 설정
    public void SetTargetSpeed(float speed)
    {
        _targetSpeed = speed;
    }

    // 현재 스피드 설정
    public void SetCurrentSpeed(float speed)
    {
        _currentSpeed = speed;
    }

    // 속도 초기화
    public void ResetSpeed()
    {
        _targetSpeed = 0f;
        _currentSpeed = 0f;
    }

    // 움직임
    public void Move()
    {
        _mover.Move(transform.forward * _currentSpeed); // 이동
    }

    // 이동 벡터를 받아서 이동
    public void Move(Vector3 velocity)
    {
        _mover.Move(velocity); // 이동
    }

    // 카메라를 기준으로 회전
    public void Rotate(bool isLerp = false, float lerpTime = 1f)
    {
        if (_mainCamera == null)
        {
            Debug.LogError("메인 카메라가 없습니다.");
            return;
        }

        Vector3 cameraForward = _mainCamera.transform.forward; // 카메라의 전방 방향
        Vector3 cameraRight = _mainCamera.transform.right; // 카메라의 우측 방향
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = cameraForward * _inputC.MoveInput.z + cameraRight * _inputC.MoveInput.x; // 카메라를 기준으로 입력 방향 계산
        direction.Normalize(); // 방향 정규화

        SetRotation(direction, isLerp, lerpTime);
    }

    public void SetRotation(Vector3 direction, bool isLerp = false, float lerpTime = 1f)
    {
        if(direction.sqrMagnitude >= 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            if (isLerp) transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * lerpTime);
            else transform.rotation = targetRotation;
        }
    }

    public void EnableGravity(bool isEnable = true)
    {
        _mover.EnableGravity = isEnable;
    }

    #endregion

    #endregion

    #region 초기화

    // 변수 초기화
    private void InitVariable()
    {
        // 속도
        _lastInputDirection = Vector3.zero;
        _currentSpeed = 0f;
        _targetSpeed = 0f;

        // 상태 체크
        _runEndTime = 0f;
        _canTurn = false;
        _isTurn = false;
    }

    // 카메라 설정
    private void InitCamera()
    {
        _mainCamera = Camera.main;
    }

    // 컴포넌트 초기화
    private void InitComponent()
    {
        TryGetComponent(out _anim);
        _anim.applyRootMotion = false;

        TryGetComponent(out _inputC);

        TryGetComponent(out _mover);
    }

    // 상태머신 초기화
    private void InitStateMachine()
    {
        _stateMachine = new PlayerStateMachine(this);
        _stateMachine.Init(_stateMachine.IdleState); // 기본 상태를 Idle로 설정
    }

    #endregion
}
