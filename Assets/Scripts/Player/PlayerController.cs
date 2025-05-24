using UnityEngine;

[RequireComponent(typeof(InputController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("평행 움직임")]
    [SerializeField] private float _walkSpeed = 3f; // 걷기 속도
    [SerializeField] private float _jogSpeed = 5f; // 조깅 속도
    [SerializeField] private float _runSpeed = 8f; // 달리기 속도
    [SerializeField] private float _fastRunSpeed = 9f; // 빠른 달리기 속도
    
    [Header("점프")]    
    [SerializeField] private float _jumpSpeed = 5f; // 점프 속도


    // 상태 머신
    private PlayerStateMachine _stateMachine;
    public PlayerStateMachine StateMachine => _stateMachine;

    // 입력
    private InputController _inputC;
    public InputController InputC => _inputC;

    // 애니메이션
    private Animator _anim;
    public Animator Anim => _anim;

    // 플레이어 무버
    private PlayerMover _mover;
    public PlayerMover Mover => _mover;
    
    private void Awake()
    {
        InitComponent();
    }

    private void Start()
    {
        InitStateMachine();
    }

    private void Update()
    {
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

    private void InitComponent()
    {
        TryGetComponent(out _anim);
        _anim.applyRootMotion = false;

        TryGetComponent(out _inputC);

        TryGetComponent(out _mover);
    }

    private void InitStateMachine()
    {
        _stateMachine = new PlayerStateMachine(this);
        _stateMachine.Init(_stateMachine.IdleState); // 기본 상태를 Idle로 설정
    }
}
