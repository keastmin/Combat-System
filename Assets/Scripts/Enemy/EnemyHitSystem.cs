using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyHitSystem : MonoBehaviour
{
    // 적이 공격을 받을 콜라이더 레이어
    [SerializeField] private LayerMask _hitColliderLayer;

    // 넉백에 저항하는 정도
    [SerializeField] private float _knockbackResistance = 0.5f;

    // 공격을 받았을 때 넉백 될 본체의 Rigidbody
    [SerializeField] private Rigidbody _bodyRigidbody;

    // 적이 공격을 받고 다음 공격을 받을 때까지의 시간
    [SerializeField] private float _hitCooldown = 0.3f;

    public void OnTriggerEnter(Collider other)
    {
        // 공격을 받은 콜라이더가 자신에게 피해를 줄 수 있는 콜라이더인지 확인
        if (((1 << other.gameObject.layer) & _hitColliderLayer) != 0)
        {
            Debug.Log("맞음");
        }
    }
}
