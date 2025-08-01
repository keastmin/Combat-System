using UnityEngine;

public class DodgeState : BaseState
{
    private float _dodgeStartTime = 0f; // 회피 시작 시간

    public DodgeState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        Debug.Log("회피 상태 진입");
        _controller.Anim.SetBool("IsDodge", true); // 애니메이션 활성화
        _dodgeStartTime = 0f; // 회피 시작 시간 초기화
        _controller.Rotate(false); // 회피 방향 설정
        _controller.SetTargetSpeed(_controller.DodgeSpeed); // 회피 속도 설정
        _controller.SetCurrentSpeed(_controller.DodgeSpeed); // 현재 속도 설정
        _controller.EnableGravity(false); // 회피 중 중력 비활성화
    }

    public override void Execute()
    {
        TransitionTo();
    }

    public override void FixedExecute()
    {
        _controller.Rotate(true, _controller.RotationSpeed);
        _controller.Move();
    }

    public override void AnimatorMove()
    {

    }

    public override void Exit()
    {
        _controller.IsTurn = false; // 회피 상태 종료 시 턴 상태는 해제

        _controller.EnableGravity(true); // 회피 종료 시 중력 활성화
        _controller.Anim.SetBool("IsDodge", false);
    }

    private void TransitionTo()
    {
        _dodgeStartTime += Time.deltaTime;

        if(_controller.InputC.JumpInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.JumpState);
        }
        else if (_controller.InputC.BasicAttackInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.AttackState);
        }
        else if (_controller.InputC.BasicAttackInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.DashAttackState);
        }
        else if (_dodgeStartTime > _controller.DodgeTime)
        {
            if (_controller.InputC.MoveInput.sqrMagnitude > 0.1f)
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
