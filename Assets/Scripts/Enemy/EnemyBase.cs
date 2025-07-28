using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public virtual void TakeDamage(int damage, Vector3 hitPoint)
    {
        Debug.Log("맞음");
    }
}
