using UnityEngine;

public class DodgeState : IState
{
    PlayerController player;

    Vector3 dir = Vector3.zero;
    Quaternion rot = Quaternion.identity;
    float time = 0f;

    public DodgeState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.StateText.text = "Current State: Dodge";
        player.TargetSpeed = player.DodgeSpeed;
        player.CurrentSpeed = player.DodgeSpeed;
        player.PlayerAnimator.SetBool("IsDodge", true);
        CalculateRotation();
        time = 0f;
    }

    public void Execute()
    {
        time += Time.deltaTime;
        if(time > player.DodgeTime)
        {
            if(player.InputController.MoveInput == Vector2.zero)
            {
                player.StateMachine.TransitionTo(player.StateMachine.idleState);
            }
            else
            {
                player.StateMachine.TransitionTo(player.StateMachine.runState);
            }
        }
    }

    public void FixedExecute()
    {
        player.transform.rotation = rot;
        player.PlayerMover.Move(dir * player.DodgeSpeed);
    }

    public void LateExecute()
    {

    }

    public void Exit()
    {
        player.PlayerAnimator.SetBool("IsDodge", false);
    }

    private void CalculateRotation()
    {
        rot = player.transform.rotation;
        dir = player.Forward * -1f;

        if(player.InputController.MoveInput != Vector2.zero)
        {
            dir = player.CalculateDirection();
            rot = Quaternion.LookRotation(dir);
        }
    }
}
