using UnityEngine;

// This is the core part of the PlayerMover class.
public partial class PlayerMover : MonoBehaviour
{
    private void MoveProcess()
    {
        _isGround = GroundDetect(out _groundInfo, _capsuleCollider.bounds.center, _rayDistance, _rayRadius, _colliderHalfHeight + _stepHeight, _detectThreshold, groundLayer);
        Debug.Log(_isGround);
        ApplyVelocity(_velocityInput);
    }

    private void ApplyVelocity(Vector3 velocity)
    {
        _rigidbody.linearVelocity = velocity;
    }
}
