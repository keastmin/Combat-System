using UnityEngine;

public class DashAttackState : IState
{
    private PlayerController _controller;
    private DashAttackData _dashAttackData;

    private float _dashAttackTime = 0f;

    public DashAttackState(PlayerController controller, PlayerAttackDataContainer attackContainer)
    {
        _controller = controller;
        _dashAttackData = attackContainer.DodgeAndRunAttackData;
    }

    public void Enter()
    {
        _controller.Anim.SetBool("IsDashAttack", true);

        _controller.SetTargetSpeed(0f);
        _controller.SetCurrentSpeed(0f);

        _dashAttackTime = 0f;
    }

    public void Execute()
    {
        TransitionTo();
    }

    public void FixedExecute()
    {

    }

    public void AnimatorMove()
    {
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Dash Attack"))
        {
            Vector3 delta = _controller.Anim.deltaPosition / Time.deltaTime;
            _controller.Move(delta);
        }
    }

    public void Exit()
    {
        _controller.Anim.SetBool("IsDashAttack", false);
        _dashAttackTime = 0f;
    }

    public void TransitionTo()
    {
        _dashAttackTime += Time.deltaTime;

        if (_dashAttackTime > _dashAttackData.EndAttackTime)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
    }
}
