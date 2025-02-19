using UnityEngine;

public class IdleState : IState
{
    PlayerController player;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.StateText.text = "Current State: Idle";
        player.TargetSpeed = 0f;
    }

    public void Execute()
    {
        if(player.InputController.MoveInput != Vector2.zero)
        {
            player.StateMachine.TransitionTo(player.StateMachine.jogState);
        }

        if (player.InputController.RunInput)
        {
            player.StateMachine.TransitionTo(player.StateMachine.dodgeState);
        }

        if (Input.GetMouseButtonDown(0))
        {
            player.StateMachine.TransitionTo(player.StateMachine.attackState);
        }
    }

    public void FixedExecute()
    {
        player.LerpSpeed(Time.fixedDeltaTime);
        player.PlayerMover.Move(player.Forward * player.CurrentSpeed);
        //player.PlayerAnimator.SetFloat("Speed", player.CurrentSpeed);
    }

    public void LateExecute()
    {

    }

    public void Exit()
    {

    }
}
