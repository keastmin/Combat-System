using JetBrains.Annotations;
using UnityEngine;

public class PlayerStateMachine
{
    public IdleState IdleState; // 기본 상태
    public WalkState WalkState; // 걷기 상태
    public JogState JogState; // 조깅 상태
    public RunState RunState; // 달리기 상태
    public TurnState TurnState; // 회전 상태
    public JumpState JumpState; // 점프 상태
    public FallState FallState; // 낙하 상태
    public DodgeState DodgeState; // 회피 상태

    private IState _prevState; // 이전 상태 (필요시 사용 가능)
    private IState _currState;
    public IState PrevState => _prevState; // 이전 상태
    public IState CurrState => _currState; // 현재 상태

    // 생성자를 통해 각 상태 초기화
    public PlayerStateMachine(PlayerController playerController)
    {
        IdleState = new IdleState(playerController);
        WalkState = new WalkState(playerController);
        JogState = new JogState(playerController);
        RunState = new RunState(playerController);
        TurnState = new TurnState(playerController);
        JumpState = new JumpState(playerController);
        FallState = new FallState(playerController);
        DodgeState = new DodgeState(playerController);
    }

    // 상태 초기화
    public void Init(IState initState)
    {
        _currState = initState;
        _currState?.Enter();
    }

    // 상태 전환
    public void Transition(IState nextState)
    {
        _currState?.Exit();
        _currState = nextState;
        _currState?.Enter();
    }

    // 상태 Update 반복
    public void Execute()
    {
        _currState?.Execute();
    }

    // 상태 Fixed Update 반복
    public void FixedExecute()
    {
        _currState?.FixedExecute();
    }

    // 상태 Animator Move 반복
    public void AnimatorMove()
    {
        _currState?.AnimatorMove();
    }
}
