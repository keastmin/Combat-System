using UnityEngine;

public class PlayerStateMachine
{
    public IdleState IdleState; // �⺻ ����
    public JogState JogState; // ���� ����

    private IState _currentState;
    public IState CurrentState => _currentState; // ���� ����

    // �����ڸ� ���� �� ���� �ʱ�ȭ
    public PlayerStateMachine(PlayerController playerController)
    {
        IdleState = new IdleState(playerController);
        JogState = new JogState(playerController);
    }

    // ���� �ʱ�ȭ
    public void Init(IState initState)
    {
        _currentState = initState;
        _currentState?.Enter();
    }

    // ���� ��ȯ
    public void Transition(IState nextState)
    {
        _currentState?.Exit();
        _currentState = nextState;
        _currentState?.Enter();
    }

    // ���� Update �ݺ�
    public void Execute()
    {
        _currentState?.Execute();
    }

    // ���� Fixed Update �ݺ�
    public void FixedExecute()
    {
        _currentState?.FixedExecute();
    }

    // ���� Animator Move �ݺ�
    public void AnimatorMove()
    {
        _currentState?.AnimatorMove();
    }
}
