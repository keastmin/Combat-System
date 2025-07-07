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

        Transform playerTransform = transform;

        if (effectObject != null)
        {
            Quaternion localRot = Quaternion.Euler(attackData.AttackEffectRotation);

            var obj = Instantiate(effectObject, playerTransform);
            obj.transform.localPosition = attackData.AttackEffectPosition;
            obj.transform.localRotation = localRot;

            ParticleSystem effect;
            obj.TryGetComponent(out effect);
            effect?.Play();
        }

        if (hitboxObject != null)
        {
            Quaternion localRot = Quaternion.Euler(attackData.HitboxRotation);
            var obj = Instantiate(hitboxObject, playerTransform);
            obj.transform.localPosition = attackData.HitboxPosition;
            obj.transform.localRotation = localRot;
            Destroy(obj, 0.1f);
        }
    }
}
