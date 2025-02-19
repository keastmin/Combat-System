using UnityEngine;

public class RunState : IState
{
    PlayerController player;

    public RunState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.StateText.text = "Current State: Run";
        player.TargetSpeed = player.RunSpeed;
    }

    public void Execute()
    {
        if(player.InputController.MoveInput == Vector2.zero)
        {
            player.StateMachine.TransitionTo(player.StateMachine.idleState);
        }

        if (player.InputController.RunInput)
        {
            player.StateMachine.TransitionTo(player.StateMachine.dodgeState);
        }
    }

    public void FixedExecute()
    {
        player.RotationBasedCamera(Time.fixedDeltaTime);
        player.LerpSpeed(Time.fixedDeltaTime);
        player.PlayerMover.Move(player.Forward * player.CurrentSpeed);
        player.PlayerAnimator.SetFloat("Speed", player.CurrentSpeed);
    }

    public void LateExecute()
    {

    }

    public void Exit()
    {

    }
}
