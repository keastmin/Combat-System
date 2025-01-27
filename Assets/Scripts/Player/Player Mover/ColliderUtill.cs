using UnityEngine;

public class ColliderUtill : MonoBehaviour
{
    public static void SetHeight(CapsuleCollider collider, float height, float stepHeight, Vector3 offset = default)
    {
        if(stepHeight > height) stepHeight = height;
        Vector3 center = offset + (Vector3.up * (height / 2f));
        center.y += stepHeight / 2f;
        collider.center = center;
        collider.height = height - stepHeight;
        LimitRadius(collider);
    }

    public static void SetRadius(CapsuleCollider collider, float radius)
    {
        collider.radius = radius;
        LimitRadius(collider);
    }

    private static void LimitRadius(CapsuleCollider collider)
    {
        if (collider.radius * 2f > collider.height) collider.radius = collider.height / 2f;
    }
}
