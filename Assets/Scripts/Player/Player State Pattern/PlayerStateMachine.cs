using UnityEngine;

public class PlayerStateMachine
{
    public IdleState IdleState; // 기본 상태
    public JogState JogState; // 조깅 상태

    private IState _currentState;
    public IState CurrentState => _currentState; // 현재 상태

    // 생성자를 통해 각 상태 초기화
    public PlayerStateMachine(PlayerController playerController)
    {
        IdleState = new IdleState(playerController);
        JogState = new JogState(playerController);
    }

    // 상태 초기화
    public void Init(IState initState)
    {
        _currentState = initState;
        _currentState?.Enter();
    }

    // 상태 전환
    public void Transition(IState nextState)
    {
        _currentState?.Exit();
        _currentState = nextState;
        _currentState?.Enter();
    }

    // 상태 Update 반복
    public void Execute()
    {
        _currentState?.Execute();
    }

    // 상태 Fixed Update 반복
    public void FixedExecute()
    {
        _currentState?.FixedExecute();
    }

    // 상태 Animator Move 반복
    public void AnimatorMove()
    {
        _currentState?.AnimatorMove();
    }
}
