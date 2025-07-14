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
        _enemySpider.EnemyNavMeshAgent.speed = _enemySpider.PatrolSpeed;
        _enemySpider.EnemyNavMeshAgent.isStopped = false;

        _targetPoint = SetTargetPoint(_enemySpider.TargetOriginPoint, _enemySpider.PatrolRadius);

        _enemySpider.EnemyNavMeshAgent.destination = _targetPoint;
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

    private Vector3 SetTargetPoint(Vector3 origin, float radius)
    {
        Vector3 result = origin;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        result.x += Mathf.Cos(angle) * distance;
        result.z += Mathf.Sin(angle) * distance;

        if (NavMesh.SamplePosition(result, out NavMeshHit hit, radius, _enemySpider.EnemyNavMeshAgent.areaMask))
        {
            result = hit.position;
        }
        else
        {
            Debug.LogWarning("경로를 찾지 못했습니다.");
        }

        return result;
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
