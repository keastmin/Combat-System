using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    protected NavMeshAgent _navMeshAgent;
    public NavMeshAgent EnemyNavMeshAgent => _navMeshAgent;

    private float _patrolRadius;
    private Vector3 _targetOriginPoint;

    public Vector3 TargetOriginPoint => _targetOriginPoint;
    public float PatrolRadius => _patrolRadius;

    protected void InitNavMesh()
    {
        if (TryGetComponent(out _navMeshAgent))
        {
            
        }
        else
        {
            Debug.LogError("NavMeshAgent 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public virtual void SetUp(Vector3 targetOriginPoint, float patrolRadius)
    {
        _targetOriginPoint = targetOriginPoint;
        _patrolRadius = patrolRadius;
    }

    public virtual void TakeDamage(int damage, Vector3 hitPoint)
    {
        Debug.Log("맞음");
    }
}
