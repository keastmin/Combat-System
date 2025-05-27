using UnityEngine;

public class DodgeState : IState
{
    private PlayerController _playerController;

    private float _dodgeStartTime = 0f; // 회피 시작 시간

    public DodgeState(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void Enter()
    {
        Debug.Log("회피 상태 진입");
        _playerController.Anim.SetBool("IsDodge", true); // 애니메이션 활성화
        _dodgeStartTime = 0f; // 회피 시작 시간 초기화
        _playerController.SetInputRotation(); // 회피 방향 설정
        _playerController.SetTargetSpeed(_playerController.DodgeSpeed); // 회피 속도 설정
        _playerController.SetCurrentSpeed(_playerController.DodgeSpeed); // 현재 속도 설정
    }

    public void Execute()
    {
        TransitionTo();
    }

    public void FixedExecute()
    {
        _playerController.Rotate();
        _playerController.Move();
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {
        _playerController.Anim.SetBool("IsDodge", false);
    }

    private void TransitionTo()
    {
        _dodgeStartTime += Time.deltaTime;

        if (_dodgeStartTime > _playerController.DodgeTime)
        {
            if (_playerController.InputC.MoveInput.sqrMagnitude > 0.1f)
            {
                _playerController.StateMachine.Transition(_playerController.StateMachine.JogState);
            }
            else
            {
                _playerController.StateMachine.Transition(_playerController.StateMachine.IdleState);
            }
        }
    }
}
