using UnityEngine;

public class AttackState : IState
{
    private PlayerController _controller;

    public AttackState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _controller.Anim.SetBool("IsAttacking", true);
    }

    public void Execute()
    {
        _controller.AttackStartToDelay += Time.deltaTime;
        if (_controller.InputC.BasicAttackInput)
        {
            _controller.Anim.SetTrigger("IsBasicAttack");
            _controller.Anim.SetFloat("IsBasicAttackTiming", _controller.AttackStartToDelay);
        }

        TransitionTo();
    }

    public void FixedExecute()
    {

    }

    public void AnimatorMove()
    {
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Attack"))
        {
            Vector3 deltaMove = _controller.Anim.deltaPosition / Time.fixedDeltaTime;
            _controller.Move(deltaMove);
        }
    }

    public void Exit()
    {
        _controller.OnSetAttackStartTime();
        _controller.Anim.SetBool("IsAttacking", false);
    }

    private void TransitionTo()
    {
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Attack"))
        {
            if(stateInfo.normalizedTime >= 0.9f)
            {
                _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
            }
        }
    }
}
