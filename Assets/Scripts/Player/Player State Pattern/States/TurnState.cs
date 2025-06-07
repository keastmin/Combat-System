using UnityEngine;

public class TurnState : IState
{
    private PlayerController _controller;
    private float _normalizedTime = 0f;
    private bool _hasTransitioned = false; // 턴 상태에서 Run 상태로 전환되었는지 여부


    public TurnState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("턴 상태 진입");
        _normalizedTime = 0f;
        _hasTransitioned = false;
        _controller.Anim.SetTrigger("IsTurn");
    }

    public void Execute()
    {
        TransitionTo();
    }

    public void FixedExecute()
    {
        _controller.Rotate(true, _controller.TurnSpeed);
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {
        _controller.IsTurn = false;
    }

    private void TransitionTo()
    {
        // TODO: 턴이 완료되면 Run 상태로 바로 이어져야함
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Turn") && !_hasTransitioned)
        {
            _normalizedTime = stateInfo.normalizedTime;

            if (_normalizedTime >= 0.8f)
            {
                _controller.StateMachine.Transition(_controller.StateMachine.RunState);
                _hasTransitioned = true;
            }
        }
        else if (_controller.InputC.DodgeInput)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.DodgeState);
        }
    }
}
