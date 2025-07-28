using UnityEngine;

public class SpiderChaseState : IState
{
    private EnemySpider _enemySpider;

    public SpiderChaseState(EnemySpider enemySpider)
    {
        this._enemySpider = enemySpider;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
        if(_enemySpider.TargetTransform != null)
        {
        }

        TransitionTo();
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

    private void TransitionTo()
    {
        if (!_enemySpider.IsTargetDetected)
        {
            _enemySpider.StateMachine.Transition(_enemySpider.StateMachine.IdleState);
        }
    }
}
