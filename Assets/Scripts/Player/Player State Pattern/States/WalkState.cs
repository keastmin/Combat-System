using UnityEngine;

public class WalkState : IState
{
    PlayerController player;

    public WalkState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.StateText.text = "Current State: Walk";
        player.TargetSpeed = player.WalkSpeed;
    }

    public void Execute()
    {
        if(player.InputController.MoveInput == Vector2.zero)
        {
            player.StateMachine.TransitionTo(player.StateMachine.idleState);
        }

        if (!player.InputController.WalkInput)
        {
            player.StateMachine.TransitionTo(player.StateMachine.jogState);
        }
    }

    public void FixedExecute()
    {
        player.RotationBasedCamera(Time.fixedDeltaTime);
        player.LerpSpeed(Time.fixedDeltaTime);
        player.PlayerMover.Move(player.Forward * player.CurrentSpeed);
    }

    public void LateExecute()
    {

    }

    public void Exit()
    {

    }
}
