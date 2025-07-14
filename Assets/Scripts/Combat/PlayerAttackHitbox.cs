using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("닿음" + other.name);
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        damageable?.TakeDamage(0, transform.position);
    }
}
