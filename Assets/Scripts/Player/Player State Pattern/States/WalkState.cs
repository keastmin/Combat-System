using UnityEngine;

public class WalkState : BaseState
{
    public WalkState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        // 이동과 관련된 애니메이션을 true로 설정
        _controller.Anim.SetBool("IsMove", true);

        // 걷기 상태에서의 속도를 설정
        _controller.SetTargetSpeed(_controller.WalkSpeed);
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
        // 이동 중에는 회전을 수행
        _controller.Rotate(true, _controller.RotationSpeed);

        // 움직임 처리
        _controller.Move();
    }

    public override void AnimatorMove()
    {

    }

    public override void Exit()
    {
        // 이동과 관련된 애니메이션을 false로 설정
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
            // 이동 입력이 없으면 Idle 상태로 전환
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
        else if (!_controller.InputC.WalkInput)
        {
            // 걷기 입력이 없고 이동 입력이 있는 경우 Jog 상태로 전환
            // Idle 상태로 전환하지 않고 Jog 상태로 바로 전환하는 이유는 Update 주기를 거쳐 전환을 처리하면
            // 애니메이터 파라미터가 IsMove를 false로 설정하여 Idle 애니메이션이 실행되기 때문
            _controller.StateMachine.Transition(_controller.StateMachine.JogState);
        }
    }
}
