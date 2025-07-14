using UnityEngine;

public class SpiderIdleState : IState
{
    private EnemySpider _enemySpider;

    private float _idlePlayTime;
    private float _patrolTime;

    public SpiderIdleState(EnemySpider enemySpider)
    {
        this._enemySpider = enemySpider;
    }

    public void Enter()
    {
        _enemySpider.EnemyNavMeshAgent.isStopped = true;

        _idlePlayTime = 0f;
        _patrolTime = Random.Range(_enemySpider.MinIdlePlayTime, _enemySpider.MaxIdlePlayTime);
    }

    public void Execute()
    {
        _idlePlayTime += Time.deltaTime;

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
        if(_idlePlayTime >= _patrolTime)
        {
            _enemySpider.StateMachine.Transition(_enemySpider.StateMachine.PatrolState);
        }
        else if (_enemySpider.IsTargetDetected)
        {
            _enemySpider.StateMachine.Transition(_enemySpider.StateMachine.ChaseState);
        }
    }
}
