using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackData", menuName = "Scriptable Objects/PlayerAttackData")]
public class PlayerAttackData : ScriptableObject
{
    [Header("Hitbox")]
    public GameObject Hitbox;
    public Vector3 HitboxPosition;
    public Vector3 HitboxRotation;

    [Header("Attack Effect")]
    public GameObject AttackEffect;
    public Vector3 AttackEffectPosition;
    public Vector3 AttackEffectRotation;
}
