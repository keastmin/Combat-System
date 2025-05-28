using UnityEngine;

public class DodgeState : IState
{
    private PlayerController _controller;

    private float _dodgeStartTime = 0f; // 회피 시작 시간

    public DodgeState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("회피 상태 진입");
        _controller.Anim.SetBool("IsDodge", true); // 애니메이션 활성화
        _dodgeStartTime = 0f; // 회피 시작 시간 초기화
        _controller.Rotate(false); // 회피 방향 설정
        _controller.SetTargetSpeed(_controller.DodgeSpeed); // 회피 속도 설정
        _controller.SetCurrentSpeed(_controller.DodgeSpeed); // 현재 속도 설정
    }

    public void Execute()
    {
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
        _controller.Anim.SetBool("IsDodge", false);
    }

    private void TransitionTo()
    {
        _dodgeStartTime += Time.deltaTime;

        if (_dodgeStartTime > _controller.DodgeTime)
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
