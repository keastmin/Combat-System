using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyHitbox : MonoBehaviour
{
    private CapsuleCollider _capsuleCollider;

    private void OnValidate()
    {
        InitComponent();
    }

    private void Awake()
    {
        OnValidate();
    }

    private void InitComponent()
    {
        TryGetComponent(out _capsuleCollider);
        _capsuleCollider.isTrigger = true;
    }
}
