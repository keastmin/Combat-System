using UnityEngine;

public class FallState : IState
{
    private PlayerController _controller;

    public FallState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("낙하 상태 진입");

    }

    public void Execute()
    {

    }

    public void FixedExecute()
    {

    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {

    }

    private void TransitionTo()
    {

    }
}
