using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttackMoveData
{
    public float MoveStartTime;
    public float MoveEndTime;
    public Vector3 MoveDirection;
    public float MoveSpeed;
}

public class ComboAttackData : ScriptableObject
{
    public string AnimationName;
    public float StateExitTime;
    public List<AttackMoveData> AttackMoveDatas;
}
