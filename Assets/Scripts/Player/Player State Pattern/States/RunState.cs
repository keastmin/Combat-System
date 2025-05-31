using UnityEngine;

public class RunState : IState
{
    private PlayerController _controller;

    private bool _isFastRun = false; // 빠른 달리기 여부
    private float _runTime = 0f; // 달리기 시간

    public RunState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("달리기 상태 진입");
        _controller.SetTargetSpeed(_controller.RunSpeed); // 달리기 속도 설정
        _isFastRun = false;
        _runTime = 0f;
    }

    public void Execute()
    {
        _controller.Anim.SetFloat("Speed", _controller.CurrentSpeed);

        if(!_isFastRun)
        {
            _runTime += Time.deltaTime;
            if(_runTime > _controller.FastRunStartTime)
            {
                _isFastRun = true;
                _controller.SetTargetSpeed(_controller.FastRunSpeed); // 빠른 달리기 속도로 설정
            }
        }

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
        _controller.CanTurn = true; // 달리기 상태 종료 후 턴이 가능한 시간을 잠깐 제공
    }

    private void TransitionTo()
    {
        if (_controller.IsTurn)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.TurnState);
        }
        else if (_controller.InputC.JumpInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.JumpState);
        }
        else if (_controller.InputC.MoveInput.sqrMagnitude <= 0.1f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
    }
}
