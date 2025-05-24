using UnityEngine;

public interface IState
{
    public void Enter();
    public void Execute();
    public void FixedExecute();
    public void AnimatorMove();
    public void Exit();
}
