using UnityEngine;

public struct GroundInfo
{
    public static GroundInfo Empty => new(false, 0f, Vector3.up, null);
    public bool IsGround;
    public float Distance;
    public Vector3 Normal;
    public Collider Collider;

    public GroundInfo(bool isGround, float distance, Vector3 normal, Collider collider)
    {
        IsGround = isGround;
        Distance = distance;
        Normal = normal;
        Collider = collider;
    }
}
