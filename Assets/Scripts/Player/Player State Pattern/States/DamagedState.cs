using UnityEngine;

public class DamagedState : IState
{
    private PlayerController _controller;

    public DamagedState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        Debug.Log("DamagedState Entered");
        _controller.Anim.SetTrigger("IsDamaged");

        // 데미지를 받는 로직
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
        Vector3 deltaPos = _controller.Anim.deltaPosition;
        Vector3 velocity = deltaPos / Time.deltaTime;
        _controller.Move(velocity);
    }

    public void Exit()
    {
        _controller.ClearDamage();
    }

    private void TransitionTo()
    {
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Damaged") && stateInfo.normalizedTime >= 0.9f)
        {
            _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
        }
    }
}
