using UnityEngine;

public class JogState : IState
{
    private PlayerController _controller;
    private PlayerStateCondition _stateCondition;

    public JogState(PlayerController controller)
    {
        _controller = controller;
        _stateCondition = _controller.StateCondition;
    }

    public void Enter()
    {
        // 이동과 관련된 애니메이션을 true로 설정
        _controller.Anim.SetBool("IsMove", true);

        // Jog 상태에서의 속도를 설정
        _controller.SetTargetSpeed(_controller.JogSpeed);
    }

    public void Execute()
    {
        // 현재 속도를 애니메이터에 전달
        _controller.Anim.SetFloat("Speed", _controller.CurrentSpeed);
        
        // 입력과 동시에 상태 전환 수행
        TransitionTo();
    }

    public void FixedExecute()
    {
        // 움직임 중에는 캐릭터가 바라보고 있는 방향으로 회전
        _controller.Rotate(true, _controller.RotationSpeed);

        // 움직임 처리
        _controller.Move();
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {
        // 이동과 관련된 애니메이션을 false로 설정
        _controller.Anim.SetBool("IsMove", false);
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
        else if (_controller.InputC.BasicAttackInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.AttackState);
        }
        else if (!_stateCondition.MoveInput)
        {
            // 이동 입력이 없으면 Idle 상태로 전환
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
        else if (_controller.InputC.WalkInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.WalkState);
        }
    }
}
