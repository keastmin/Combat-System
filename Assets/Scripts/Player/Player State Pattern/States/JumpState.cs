using UnityEngine;

public class JumpState : IState
{
    private PlayerController _controller;

    public JumpState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _controller.Anim.SetTrigger("IsJump");

        _controller.Jump(new Vector3(0, _controller.JumpSpeed, 0));
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
        Transition();
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
        
    }

    private void Transition()
    {
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Jump Start") && stateInfo.normalizedTime >= 1.0f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.FallState);
        }
    }
}
