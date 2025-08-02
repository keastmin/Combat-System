using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        _controller.Anim.SetTrigger("IsJump");

        _controller.Jump(new Vector3(0, _controller.JumpSpeed, 0));
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
        Transition();
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
