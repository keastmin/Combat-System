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
        _controller.Anim.SetBool("IsFalling", true);
    }

    public void Execute()
    {
        if (_controller.InputC.MoveInput.sqrMagnitude > 0.1f && _controller.CurrentSpeed >= _controller.RunSpeed - 1.0f)
        {
            _controller.SetTargetSpeed(_controller.RunSpeed);
        }
        else if (_controller.InputC.MoveInput.sqrMagnitude > 0.1f && _controller.CurrentSpeed >= _controller.JogSpeed - 1.0f)
        {
            _controller.SetTargetSpeed(_controller.JogSpeed);
        }
        else if (_controller.InputC.MoveInput.sqrMagnitude > 0.1f)
        {
            _controller.SetTargetSpeed(_controller.WalkSpeed);
        }
        else if (_controller.InputC.MoveInput.sqrMagnitude < 0.1f)
        {
            _controller.SetTargetSpeed(0f);
        }
        TransitionTo();
    }

    public void FixedExecute()
    {
        _controller.Move();
        _controller.Rotate(true, _controller.RotationSpeed);
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {
        _controller.IsTurn = false; // 낙하 상태 종료 시 턴 상태는 해제

        _controller.Anim.SetBool("IsFalling", false);
    }

    private void TransitionTo()
    {
        if (_controller.Mover.IsOnGround)
        {
            if (_controller.CurrentSpeed >= _controller.RunSpeed - 0.1f)
            {
                _controller.StateMachine.Transition(_controller.StateMachine.RunState);
            }
            else
            {
                _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
            }
        }
    }
}
