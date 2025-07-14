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
        _enemySpider.EnemyNavMeshAgent.speed = _enemySpider.ChaseSpeed;
        _enemySpider.EnemyNavMeshAgent.isStopped = false;
    }

    public void Execute()
    {
        if(_enemySpider.TargetTransform != null)
        {
            _enemySpider.EnemyNavMeshAgent.SetDestination(_enemySpider.TargetTransform.position);
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
