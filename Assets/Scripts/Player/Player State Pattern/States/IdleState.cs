using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class IdleState : IState
{
    private PlayerController _controller;

    public IdleState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("Idle 상태 진입");
        _controller.SetTargetSpeed(0f);
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
        else if (_controller.InputC.MoveInput.sqrMagnitude > 0.1f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.JogState);
        }
    }
}
