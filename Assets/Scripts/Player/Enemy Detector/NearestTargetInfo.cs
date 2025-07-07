using UnityEngine;

public struct NearestEnemyInfo
{
    public static NearestEnemyInfo Empty => new NearestEnemyInfo(null, Vector3.zero, false);
    public Collider Collider;
    public Vector3 Point;
    public bool InRange;

    public NearestEnemyInfo(Collider collider, Vector3 point, bool inRange)
    {
        Collider = collider;
        Point = point;
        InRange = inRange;
    }
}
