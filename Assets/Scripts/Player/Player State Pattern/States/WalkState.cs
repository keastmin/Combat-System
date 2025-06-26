using UnityEngine;

public class WalkState : IState
{
    private PlayerController _controller;

    public WalkState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _controller.Anim.SetBool("IsMove", true);
        _controller.SetTargetSpeed(_controller.WalkSpeed);
    }

    public void Execute()
    {
        _controller.Anim.SetFloat("Speed", _controller.CurrentSpeed);

        TransitionTo();
    }

    public void FixedExecute()
    {
        _controller.Rotate(true, _controller.RotationSpeed);
        _controller.Move();
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {
        _controller.Anim.SetBool("IsMove", false);
    }

    private void TransitionTo()
    {
        if (_controller.InputC.JumpInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.JumpState);
        }
        else if (!_controller.Mover.IsOnGround)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.FallState);
        }
        else if (_controller.InputC.BasicAttackInput) 
        {
             _controller.StateMachine.Transition(_controller.StateMachine.AttackState);
        }
        else if (_controller.InputC.MoveInput.sqrMagnitude <= 0.1f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
        else if (!_controller.InputC.WalkInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.JogState);
        }
    }
}
