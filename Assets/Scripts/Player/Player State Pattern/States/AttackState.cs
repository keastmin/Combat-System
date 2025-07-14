using UnityEditor;
using UnityEngine;

public class AttackState : IState
{
    private PlayerController _controller;
    private PlayerAttackDataContainer _attackDataContainer;

    private Vector3 _target;
    private int _currentComboIndex = 0;
    private float _attackStartToTime = 0f;
    private float _attackStartToFixedTime = 0f;

    public AttackState(PlayerController controller, PlayerAttackDataContainer attackDataContainer)
    {
        _controller = controller;
        _attackDataContainer = attackDataContainer;
    }

    public void Enter()
    {
        Debug.Log("AttackState 진입, Combo: " + (_currentComboIndex + 1));

        _controller.Anim.SetBool("IsAttacking", true);
        _controller.Anim.SetTrigger("IsNextAttack");
        _attackStartToTime = 0f;
        _attackStartToFixedTime = 0f;

        _controller.SetTargetSpeed(0f);
        _controller.SetCurrentSpeed(0f);

        if (_controller.NearestEnemy.InRange)
        {
            _target = _controller.NearestEnemy.Point - _controller.transform.position;
            _target.y = 0f;
        }
        else
            _target = _controller.transform.forward * 1.3f;
    }

    public void Execute()
    {
        TransitionTo();
    }

    public void FixedExecute()
    {
        _attackStartToFixedTime += Time.fixedDeltaTime;
        Vector3 vel = Vector3.zero;
        if (_attackStartToFixedTime <= 0.1f)
        {
            vel = _target / 0.1f;
            if (_controller.NearestEnemy.InRange)
                _controller.SetRotation(_target, true, 20f);
        }

        Vector3 deltaMove = _controller.Anim.deltaPosition / Time.fixedDeltaTime;

        // 최종 이동 적용
        _controller.Move(vel);
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {
        _attackStartToTime = 0f;
        _attackStartToFixedTime = 0f;
    }

    private void TransitionTo()
    {
        _attackStartToTime += Time.deltaTime;
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Attack"))
        {
            if (stateInfo.normalizedTime >= _attackDataContainer.Basic3ComboAttackDatas[_currentComboIndex].CanMoveStartTime)
            {
                _controller.Anim.SetBool("IsAttacking", false);
                _currentComboIndex = 0;
                _controller.StateMachine.Transition(_controller.StateMachine.IdleState);
            }
            else if (stateInfo.normalizedTime > _attackDataContainer.Basic3ComboAttackDatas[_currentComboIndex].CanNextAttackStartTime &&
                    stateInfo.normalizedTime < _attackDataContainer.Basic3ComboAttackDatas[_currentComboIndex].CanNextAttackEndTime &&
                    _controller.InputC.BasicAttackInput && _currentComboIndex < _attackDataContainer.Basic3ComboAttackDatas.Length - 1)
            {
                _currentComboIndex++;
                _controller.StateMachine.Transition(_controller.StateMachine.AttackState);
            }
            else if (_controller.InputC.DodgeInput)
            {
                _controller.Anim.SetBool("IsAttacking", false);
                _currentComboIndex = 0;
                _controller.StateMachine.Transition(_controller.StateMachine.DodgeState);
            }
        }
    }
}
