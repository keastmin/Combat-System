using UnityEngine;

public class AttackState : IState
{
    PlayerController player;

    AnimatorClipInfo[] clipInfo;

    bool nextCombo = false;

    public AttackState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.StateText.text = "Current State: Attack";
        player.PlayerMover.ClearVelocity();

        player.PlayerAnimator.SetTrigger("IsAttack");
        nextCombo = false;
    }

    public void Execute()
    {
        AnimatorStateInfo stateInfo = player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);
        float normalTime = stateInfo.normalizedTime;
        float aniSpeed = player.PlayerAnimator.GetFloat("AniSpeed");

        if (aniSpeed == 0)
        {
            player.PlayerAnimator.speed = 1;
        }
        else
        {
            player.PlayerAnimator.speed = aniSpeed;
        }

        if (Input.GetMouseButtonDown(0))
        {
            player.PlayerAnimator.SetTrigger("IsAttack");
            player.PlayerAnimator.SetFloat("IsAttackTiming", normalTime);
        }

        if (normalTime >= 0.9f && !nextCombo)
        {
            player.StateMachine.TransitionTo(player.StateMachine.idleState);
        }
    }

    public void FixedExecute()
    {

    }

    public void LateExecute()
    {

    }

    public void Exit()
    {
        player.PlayerAnimator.speed = 1f;
        player.PlayerAnimator.ResetTrigger("IsAttack");
    }
}
