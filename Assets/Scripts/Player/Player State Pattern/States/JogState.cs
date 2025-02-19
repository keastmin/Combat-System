using UnityEngine;

public class JogState : IState
{
    PlayerController player;

    public JogState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.StateText.text = "Current State: Jog";
        player.TargetSpeed = player.JogSpeed;
    }

    public void Execute()
    {
        if (player.InputController.MoveInput == Vector2.zero)
        {
            player.StateMachine.TransitionTo(player.StateMachine.idleState);
        }

        if (player.InputController.WalkInput)
        {
            player.StateMachine.TransitionTo(player.StateMachine.walkState);
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
