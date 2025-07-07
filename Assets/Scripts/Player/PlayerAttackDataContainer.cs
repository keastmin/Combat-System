using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttackDataDictionary
{
    public string AttackName;
    public PlayerAttackData PlayerAttackData;
}

public class PlayerAttackDataContainer : MonoBehaviour
{
    public BasicAttackData[] Basic3ComboAttackDatas;
    public DashAttackData DashAttackData;

    [SerializeField] private List<AttackDataDictionary> _attackDatas;

    private Dictionary<string, PlayerAttackData> _attackDataDictionary;

    private void Awake()
    {
        _attackDataDictionary = new Dictionary<string, PlayerAttackData>();
        foreach(var attackData in _attackDatas)
        {
            _attackDataDictionary[attackData.AttackName] = attackData.PlayerAttackData;
        }
    }

    public void OnAttackEffectAnimationEvent(string attackName)
    {
        var attackData = _attackDataDictionary[attackName];

        if (attackData == null) return;

        var effectObject = attackData.AttackEffect;
        var hitboxObject = attackData.Hitbox;

        Transform playerTransform = transform; // 혹은 _controller.transform 등 네 플레이어 Transform

        if (effectObject != null)
        {
            // 로컬 Position → 월드 Position
            Vector3 worldPos = playerTransform.TransformPoint(attackData.AttackEffectPosition);

            // 로컬 Rotation → 월드 Rotation
            Quaternion localRot = Quaternion.Euler(attackData.AttackEffectRotation);
            Quaternion worldRot = playerTransform.rotation * localRot;

            var obj = Instantiate(effectObject, worldPos, worldRot);

            ParticleSystem effect;
            obj.TryGetComponent(out effect);
            effect?.Play();

            Destroy(obj, 5f);

            // Pool로 바꾸고 싶으면:
            // var obj = poolManager.GetFromPool(effectKey, effectObject, worldPos, worldRot);
        }

        if (hitboxObject != null)
        {
            Vector3 worldPos = playerTransform.TransformPoint(attackData.HitboxPosition);
            Quaternion localRot = Quaternion.Euler(attackData.HitboxRotation);
            Quaternion worldRot = playerTransform.rotation * localRot;

            var obj = Instantiate(hitboxObject, worldPos, worldRot);
            Destroy(obj, 5f);

            // Pool 버전:
            // var obj = poolManager.GetFromPool(hitboxKey, hitboxObject, worldPos, worldRot);
        }
    }
}
