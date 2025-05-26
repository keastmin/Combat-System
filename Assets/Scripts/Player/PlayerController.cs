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

    [Header("회전")]
    [SerializeField] private float _rotationSpeed = 10f; // 회전 속도

    [Header("점프")]    
    [SerializeField] private float _jumpSpeed = 5f; // 점프 속도


    // 필드 프로퍼티
    public float WalkSpeed => _walkSpeed; // 걷기 속도
    public float JogSpeed => _jogSpeed; // 조깅 속도

    // 속도
    private Vector3 _lastInputDirection; // 마지막 이동 입력 방향
    private float _currentSpeed; // 현재 속도
    private float _targetSpeed; // 목표 속도
    public float CurrentSpeed => _currentSpeed; // 현재 속도 프로퍼티

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

    private void Update()
    {
        LerpCurrentSpeed(_targetSpeed, _currentSpeed, _speedLerpTime);
        _stateMachine?.Execute();
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
        // if (_inputC.MoveInput.sqrMagnitude > 0.1f) _lastInputDirection = _inputC.MoveInput; // 마지막 입력 방향 설정
        _mover.Move(transform.forward * _currentSpeed); // 이동
    }

    // 이동 벡터를 받아서 이동
    public void Move(Vector3 velocity)
    {
        _mover.Move(velocity); // 이동
    }

    // 카메라를 기준으로 회전
    public void Rotate()
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

        if(direction.sqrMagnitude >= 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction); // 목표 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
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
