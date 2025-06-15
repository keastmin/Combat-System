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
    [SerializeField] private Image _targetMarkImage;

    [Header("Hard Targeting")]
    [SerializeField] private float _hardTargetingRange = 2f;

    [Header("Soft Targeting")]
    [SerializeField] private float _softTargetingRange = 4f;
    [SerializeField] private float _softTargetingAngle = 150f;

    [Header("Debug")]
    [SerializeField] private bool _debugHardTargetZone = false;

    public float MaxTargetingDistance => _hardTargetingRange;

    private NearestEnemyInfo _neareastEnemy = NearestEnemyInfo.Empty;
    public NearestEnemyInfo NearestEnemy => _neareastEnemy;

    // 감지된 타겟 콜라이더
    private Collider[] _overlapTargetColliders;

    private void Awake()
    {
        _overlapTargetColliders = new Collider[15]; // 최대 10개의 적을 감지할 수 있도록 설정
    }

    private void Update()
    {
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        NearestEnemyInfo newNearestEnemy = NearestEnemyInfo.Empty;
        bool hardTargeting = false;

        hardTargeting = NearTargeting(out newNearestEnemy);

        _neareastEnemy = newNearestEnemy;
    }

    // 플레이어를 기준으로 360도 회전하는 전체 방향에서 적대적이거나 이미 전투 중인 적 중 가장 가까운 적을 찾습니다.
    private bool NearTargeting(out NearestEnemyInfo nearestEnemy)
    {
        nearestEnemy = NearestEnemyInfo.Empty;

        Vector3 origin = transform.position;
        Vector3 parellelOrigin = new Vector3(origin.x, 0f, origin.z);
        float radius = _hardTargetingRange;
        int detectedCount = 0;
        float minParellelDistance = 5f;

        detectedCount = Physics.OverlapSphereNonAlloc(origin, radius, _overlapTargetColliders, _targetLayerMask);

        if (detectedCount > 0)
        {
            foreach(var targetCollider in _overlapTargetColliders)
            {
                if(targetCollider != null)
                {
                    Vector3 parellelTargetPosition = new Vector3(targetCollider.transform.position.x, 0f, targetCollider.transform.position.z);
                    float distance = Vector3.Distance(parellelTargetPosition, parellelOrigin);

                    if(minParellelDistance > distance)
                    {
                        nearestEnemy.Collider = targetCollider;
                        nearestEnemy.Point = targetCollider.transform.position;
                        nearestEnemy.ParellelDistance = distance;
                        nearestEnemy.InRange = true;
                    }
                }
            }

            return true;
        }

        return false;
    }

    // 공격을 이어가고자 하는 락온, 하나의 공격 사이클이 끝나면 자동 해제
    private void AttackLockOn()
    {

    }

    // 사용자가 직접 락온 하여 거리가 멀어지지 않는 한 취소되지 않는 락온
    private void ToggleLockOn()
    {

    }

    private void OnDrawGizmos()
    {
        if (_debugHardTargetZone)
        {
            Gizmos.DrawWireSphere(transform.position, _hardTargetingRange);
        }
    }
}
