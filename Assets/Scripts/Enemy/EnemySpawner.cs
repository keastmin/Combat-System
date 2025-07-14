using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _enemyCount = 5;
    [SerializeField] private float _spawnRadius = 5f;
    [SerializeField] private float _spawnSpaceBetweenEnemies = 2f;

    [Header("Enemy Settings")]
    [SerializeField] private float _patrolRadius = 5f;

    [Header("Debug")]
    [SerializeField] private bool _debugRadius = true;

    private void Start()
    {
        for(int i = 0; i < _enemyCount; i++)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        // 적 생성
        GameObject enemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);

        // EnemyBase 클래스 가져오고 설정하기
        enemy.TryGetComponent(out EnemyBase enemyBase);
        enemyBase.SetUp(transform.position, _patrolRadius);

        // 스폰 위치 계산
        Vector3 spawnPosition = transform.position;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * _spawnRadius;

        spawnPosition.x += Mathf.Cos(angle) * distance;
        spawnPosition.z += Mathf.Sin(angle) * distance;

        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, _spawnRadius, enemyBase.EnemyNavMeshAgent.areaMask))
        {
            spawnPosition = hit.position;
        }
        else
        {
            Debug.LogWarning("경로를 찾지 못했습니다.");
        }

        // 스폰 위치 설정
        enemy.transform.position = spawnPosition;
    }

    private void OnDrawGizmos()
    {
        if (_debugRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);
        }
    }
}
