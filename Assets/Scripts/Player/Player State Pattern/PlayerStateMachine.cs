using UnityEngine;

public class PlayerStateMachine
{
    public IState CurrentState { get; private set; }

    public IdleState idleState;
    public WalkState walkState;
    public JogState jogState;
    public RunState runState;
    public DodgeState dodgeState;
    public AttackState attackState;

    public PlayerStateMachine(PlayerController player)
    {
        idleState = new IdleState(player);
        walkState = new WalkState(player);
        jogState = new JogState(player);
        runState = new RunState(player);
        dodgeState = new DodgeState(player);
        attackState = new AttackState(player);
    }

    public void InitState(IState initState)
    {
        CurrentState = initState;
        CurrentState.Enter();
    }

    public void TransitionTo(IState nextState)
    {
        if(CurrentState != null)
        {
            CurrentState.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }
    }

    public void Execute()
    {
        if(CurrentState != null)
        {
            CurrentState.Execute();
        }
    }

    public void FixedExecute()
    {
        if (CurrentState != null)
        {
            CurrentState.FixedExecute();
        }
    }

    public void LateExecute()
    {
        if (CurrentState != null)
        {
            CurrentState.LateExecute();
        }
    }
}
