using UnityEngine;

public class JumpState : IState
{
    private PlayerController _controller;

    public JumpState(PlayerController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _controller.Anim.SetTrigger("IsJump");

        _controller.Jump(new Vector3(0, _controller.JumpSpeed, 0));
    }

    public void Execute()
    {
        Transition();
    }

    public void FixedExecute()
    {
        
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {
        
    }

    private void Transition()
    {
        AnimatorStateInfo stateInfo = _controller.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Jump Start") && stateInfo.normalizedTime >= 1.0f)
        {

        }
    }
}
