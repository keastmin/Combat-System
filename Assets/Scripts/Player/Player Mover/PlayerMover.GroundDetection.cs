using UnityEngine;

// This is the ground detection part of the PlayerMover class.
public partial class PlayerMover : MonoBehaviour
{
    public bool GroundDetect(out GroundInfo groundInfo, Vector3 rayPosition, float rayDistance, float rayRadius, float detectDistance, float detectThreshold, LayerMask layer)
    {
        bool groundDetected = false;
        RaycastHit hit;
        groundInfo = GroundInfo.Empty;

        if(rayRadius <= 0f)
        {
            groundDetected = Physics.Raycast(rayPosition, Vector3.down, out hit, rayDistance, layer);
        }
        else
        {
            groundDetected = Physics.SphereCast(rayPosition, rayRadius, Vector3.down, out hit, rayDistance, layer);
        }

        if (groundDetected)
        {
            groundInfo.IsGround = hit.distance <= (detectDistance + detectThreshold);
            groundInfo.Distance = hit.distance;
            groundInfo.Normal = hit.normal;
            groundInfo.Collider = hit.collider;

            if(rayRadius <= 0f)
            {
                Vector3 point = hit.point + Vector3.up * 0.1f;
                if(Physics.Raycast(point, Vector3.down, out hit, rayDistance, layer))
                {
                    groundInfo.Normal = hit.normal;
                }
            }
        }

        return groundInfo.IsGround;
    }
}
