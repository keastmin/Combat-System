using UnityEngine;

public enum CommandType
{
    Move, Walk, Jump, Dodge, BasicAttack, Skill1, Skill2, Skill3, Parry
}

public struct InputCommand
{
    public readonly CommandType CommandType;
    public readonly float InputTime;
    public readonly float BufferTime;

    public InputCommand(CommandType type, float bufferTime)
    {
        CommandType = type;
        InputTime = Time.time;
        BufferTime = bufferTime;
    }

    public bool IsExpired => Time.time - InputTime > BufferTime;
}
