using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [SerializeField] private bool _proactive = false; // 적이 플레이어를 먼저 공격하는지 여부
    [SerializeField] private bool _aggressive = false; // 적이 플레이어를 공격하는지 여부
    [SerializeField] private bool _inCombat = false; // 적이 전투 상태에 돌입했는지 여부

    public bool InCombat => _inCombat;

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        _inCombat = true;
    }
}
