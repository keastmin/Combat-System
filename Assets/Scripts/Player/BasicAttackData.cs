using UnityEngine;

[CreateAssetMenu(fileName = "BasicAttackData", menuName = "Scriptable Objects/BasicAttackData")]
public class BasicAttackData : ScriptableObject
{
    public float CanNextAttackStartTime;
    public float CanNextAttackEndTime;
    public float CanMoveStartTime;
}
