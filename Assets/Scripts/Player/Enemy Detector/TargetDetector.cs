using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hard Targeting: 나에게 적대적이거나 전투 중인 적에게 강제로 타겟팅을 시도합니다
/// Soft Targeting: 플레이어가 바라보는 방향에 있는 적을 타겟팅합니다
/// </summary>
public class TargetDetector : MonoBehaviour
{
    [Header("Target Information")]
    [SerializeField] private LayerMask _targetLayerMask;

    [Header("Targeting")]
    [SerializeField] private float _targetingRange = 2f;
    [SerializeField] private int _maxDetectCount = 15; // 최대 감지 가능한 적의 수

    [Header("Debug")]
    [SerializeField] private bool _debugHardTargetZone = false;

    public float MaxTargetingDistance => _targetingRange;

    private NearestEnemyInfo _nearestEnemy = NearestEnemyInfo.Empty;
    public NearestEnemyInfo NearestEnemy => _nearestEnemy;

    // 감지된 타겟 콜라이더
    private Collider[] _overlapTargetColliders;

    private void Awake()
    {
        _nearestEnemy = NearestEnemyInfo.Empty;
        _overlapTargetColliders = new Collider[_maxDetectCount]; // 최대 감지 가능한 적의 수
    }

    private void Update()
    {
        _nearestEnemy = NearTargeting();

        // 초기화
        for(int i = 0; i < _maxDetectCount; i++)
        {
            if (_overlapTargetColliders[i] != null)
            {
                _overlapTargetColliders[i] = null;
            }
        }
    }

    // 플레이어를 기준으로 360도 회전하는 전체 방향에서 적대적이거나 이미 전투 중인 적 중 가장 가까운 적을 찾습니다.
    private NearestEnemyInfo NearTargeting()
    {
        NearestEnemyInfo newNearestEnemyInfo = NearestEnemyInfo.Empty;
        Vector3 origin = transform.position;
        float radius = _targetingRange;
        int detectedCount = 0;
        float minDistance = _targetingRange + 1f;

        detectedCount = Physics.OverlapSphereNonAlloc(origin, radius, _overlapTargetColliders, _targetLayerMask);

        if (detectedCount > 0)
        {
            foreach(var targetCollider in _overlapTargetColliders)
            {
                if(targetCollider != null)
                {
                    float distance = Vector3.Distance(targetCollider.transform.position, origin);

                    if(minDistance > distance)
                    {
                        minDistance = distance;

                        newNearestEnemyInfo.Collider = targetCollider;
                        newNearestEnemyInfo.Point = targetCollider.transform.position;
                        newNearestEnemyInfo.InRange = true;
                    }
                }
            }
        }

        return newNearestEnemyInfo;
    }

    private void OnDrawGizmos()
    {
        if (_debugHardTargetZone)
        {
            Gizmos.DrawWireSphere(transform.position, _targetingRange);
        }
    }
}
