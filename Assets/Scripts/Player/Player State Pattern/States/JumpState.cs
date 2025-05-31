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
        Debug.Log("점프 상태 진입");
    }

    public void Execute()
    {
        
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
}
