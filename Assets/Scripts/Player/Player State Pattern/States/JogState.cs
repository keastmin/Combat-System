using UnityEngine;

public class JogState : IState
{
    private PlayerController _controller;

    public JogState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _controller.SetTargetSpeed(_controller.JogSpeed);
    }

    public void Execute()
    {
        _controller.Anim.SetFloat("Speed", _controller.CurrentSpeed);

        TransitionTo();
    }

    public void FixedExecute()
    {
        _controller.Move();
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {

    }

    private void TransitionTo()
    {
        if(_controller.InputC.MoveInput.sqrMagnitude < 0.1f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
    }
}
