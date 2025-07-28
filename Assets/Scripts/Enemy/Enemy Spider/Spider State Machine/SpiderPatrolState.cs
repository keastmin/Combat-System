using UnityEngine;
using UnityEngine.AI;

public class SpiderPatrolState : IState
{
    private EnemySpider _enemySpider;

    private Vector3 _targetPoint;

    public SpiderPatrolState(EnemySpider enemySpider)
    {
        this._enemySpider = enemySpider;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
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

    private float GetDistanceToTarget(Vector3 targetPoint)
    {
        return Vector3.Distance(_enemySpider.transform.position, targetPoint);
    }

    private void TransitionTo()
    {
        if(GetDistanceToTarget(_targetPoint) <= _enemySpider.TargetPointThreshold)
        {
            _enemySpider.StateMachine.Transition(_enemySpider.StateMachine.IdleState);
        }
        else if (_enemySpider.IsTargetDetected)
        {
            _enemySpider.StateMachine.Transition(_enemySpider.StateMachine.ChaseState);
        }
    }
}
