using System;
using UnityEngine;

[Serializable]
public class IdleState : BaseState
{
    public IdleState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        // Idle 상태에서의 속도를 설정
        _controller.SetTargetSpeed(0f);
    }

    public override void Execute()
    {
        // 현재 속도를 애니메이터에 전달
        _controller.Anim.SetFloat("Speed", _controller.CurrentSpeed);

        // 입력과 동시에 상태 전환 수행
        TransitionTo();
    }

    public override void FixedExecute()
    {
        // 움직임 처리, Idle 상태에서도 걷거나 뛰다가 서서히 0으로 감속하는 시간이 필요할 수 있음
        _controller.Move();
    }

    public override void AnimatorMove()
    {

    }

    public override void Exit()
    {

    }

    private void TransitionTo()
    {
        if (_controller.InputC.BasicAttackInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.AttackState);
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
        else if (_condition.MoveInput)
        {
            // 이동 입력이 있을 경우 Jog 상태로 전환
            _controller.StateMachine.Transition(_controller.StateMachine.JogState);
        }
    }
}
