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
        Debug.Log("조깅 상태 진입");
        _controller.SetTargetSpeed(_controller.JogSpeed);
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

    }

    private void TransitionTo()
    {
        if (_controller.InputC.DodgeInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.DodgeState);
        }
        else if (_controller.IsTurn)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.TurnState);
        }
        else if (_controller.InputC.JumpInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.JumpState);
        }
        else if (!_controller.Mover.IsOnGround)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.FallState);
        }
        else if (_controller.InputC.MoveInput.sqrMagnitude < 0.1f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
        else if (_controller.InputC.WalkInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.WalkState);
        }
    }
}
