using UnityEngine;

[RequireComponent(typeof(InputController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("���� ������")]
    [SerializeField] private float _walkSpeed = 3f; // �ȱ� �ӵ�
    [SerializeField] private float _jogSpeed = 5f; // ���� �ӵ�
    [SerializeField] private float _runSpeed = 8f; // �޸��� �ӵ�
    [SerializeField] private float _fastRunSpeed = 9f; // ���� �޸��� �ӵ�
    
    [Header("����")]    
    [SerializeField] private float _jumpSpeed = 5f; // ���� �ӵ�


    // ���� �ӽ�
    private PlayerStateMachine _stateMachine;
    public PlayerStateMachine StateMachine => _stateMachine;

    // �Է�
    private InputController _inputC;
    public InputController InputC => _inputC;

    // �ִϸ��̼�
    private Animator _anim;
    public Animator Anim => _anim;

    // �÷��̾� ����
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
        _stateMachine.Init(_stateMachine.IdleState); // �⺻ ���¸� Idle�� ����
    }
}
