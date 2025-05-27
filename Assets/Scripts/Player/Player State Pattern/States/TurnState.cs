using UnityEngine;

public class TurnState : IState
{
    private PlayerController _controller;

    public TurnState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("턴 상태 진입");
        _controller.Anim.SetTrigger("IsTurn");
    }

    public void Execute()
    {
        TransitionTo();
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
