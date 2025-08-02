using UnityEngine;

public class FallState : BaseState
{
    public FallState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        Debug.Log("낙하 상태 진입");
        _controller.Anim.SetBool("IsFalling", true);
    }

    public override void Execute()
    {
        if (_controller.InputC.MoveInput.sqrMagnitude > 0.1f && _controller.CurrentSpeed > _controller.JogSpeed + 0.1f)
        {
            _controller.SetTargetSpeed(_controller.RunSpeed);
        }
        else if (_controller.InputC.MoveInput.sqrMagnitude > 0.1f)
        {
            _controller.SetTargetSpeed(_controller.JogSpeed);
        }
        else if (_controller.InputC.MoveInput.sqrMagnitude < 0.1f)
        {
            _controller.SetTargetSpeed(0f);
        }
        TransitionTo();
    }

    public override void FixedExecute()
    {
        _controller.Move();
        _controller.Rotate(true, _controller.RotationSpeed);
    }

    public override void AnimatorMove()
    {

    }

    public override void Exit()
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
