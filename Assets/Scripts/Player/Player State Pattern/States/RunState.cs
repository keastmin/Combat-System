using UnityEngine;

public class RunState : IState
{
    private PlayerController _controller;

    public RunState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("달리기 상태 진입");
    }

    public void Execute()
    {
        _controller.Anim.SetFloat("Speed", _controller.CurrentSpeed);

        TransitionTo();
    }

    public void FixedExecute()
    {
        _controller.Rotate();
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
        if (_controller.InputC.MoveInput.sqrMagnitude < 0.1f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
    }
}
