using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AttackState : IState
{
    private PlayerController _controller;

    private Vector3 _target;

    public AttackState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _controller.Anim.SetBool("IsAttacking", true);

        _controller.SetTargetSpeed(0f);
        _controller.SetCurrentSpeed(0f);

        Debug.Log(_controller.NearestEnemy.InRange);

        if (_controller.NearestEnemy.InRange)
        {
            _target = _controller.NearestEnemy.Point - _controller.transform.position;
            _controller.transform.rotation = Quaternion.LookRotation(_target);
        }
        else
        {
            _target = _controller.transform.forward * 1.3f;
        }
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
        //if (stateInfo.IsTag("Attack"))
        //{
            Vector3 vel = Vector3.zero;
            if (_controller.AttackStartToDelay <= 0.1f)
            {
                vel = _target / 0.1f;
            }

            Vector3 deltaMove = _controller.Anim.deltaPosition / Time.fixedDeltaTime;

            // 최종 이동 적용
            _controller.Move(vel);
        //}
    }

    public void AnimatorMove()
    {
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);


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
