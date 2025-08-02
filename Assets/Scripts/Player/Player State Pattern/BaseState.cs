using System;
using UnityEngine;

[Serializable]
public abstract class BaseState
{
    public int Priority;

    protected PlayerController _controller;
    protected PlayerStateCondition _condition;

    public BaseState(PlayerController controller)
    {
        _controller = controller;
        _condition = _controller.StateCondition;
    }

    public abstract void Enter();
    public abstract void Exit();

    public virtual void Execute()
    {

    }

    public virtual void FixedExecute()
    {

    }

    public virtual void AnimatorMove()
    {

    }
}
