using UnityEngine;

public struct NearestEnemyInfo
{
    public static NearestEnemyInfo Empty => new NearestEnemyInfo(null, Vector3.zero, 0f, false);
    public Collider Collider;
    public Vector3 Point;
    public float ParellelDistance;
    public bool InRange;

    public NearestEnemyInfo(Collider collider, Vector3 point, float parellelDistance, bool inRange)
    {
        Collider = collider;
        Point = point;
        ParellelDistance = parellelDistance;
        InRange = inRange;
    }
}
