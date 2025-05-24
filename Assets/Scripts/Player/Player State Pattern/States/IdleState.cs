using UnityEngine;

public class IdleState : IState
{
    private PlayerController _controller;

    public IdleState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {

    }

    public void Execute()
    {
        _controller.Anim.SetFloat("Speed", _controller.InputC.MoveInput.sqrMagnitude);

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
        if(_controller.InputC.MoveInput.sqrMagnitude > 0.1f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.JogState);
        }
    }
}
