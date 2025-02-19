using Unity.VisualScripting;
using UnityEngine;

// This is the core part of the PlayerMover class.
public partial class PlayerMover : MonoBehaviour
{
    private void MoveProcess(float delta)
    {
        InitValiable();
        _isGround = GroundDetect(out _groundInfo, _capsuleCollider.bounds.center, _rayDistance, _rayRadius, _colliderHalfHeight + _stepHeight, groundLayer);

        if (_isJumping && _yAxisVelocity <= 0f) _isJumping = false;
        ApplyGravityVelocity(delta);

        Vector3 stepHover = StepHover();
        ApplyVelocity(_velocityInput + stepHover);
    }

    private void InitValiable()
    {
        _calculatedVelocity = Vector3.zero;
        _groundInfo = GroundInfo.Empty;
        _isGround = false;
    }

    private void ApplyGravityVelocity(float delta)
    {
        if (UseGravity)
        {
            if (!IsGround || _isJumping)
            {
                _gravityForce += (_gravityAcc * delta);
                if (_gravityForce > _maxFallSpeed) _gravityForce = _maxFallSpeed;

                _yAxisVelocity -= _gravityForce * delta;
            }
            else
            {
                _gravityForce = 0f;
                _yAxisVelocity = 0f;
            }
        }
    }

    private Vector3 StepHover()
    {
        Vector3 vel = Vector3.zero;
        float stepDistance = _colliderHalfHeight + _stepHeight;

        if (Mathf.Abs(stepDistance - _groundInfo.Distance) < _stepHeight && !_isJumping)
        {
            float remainingDistance = stepDistance - _groundInfo.Distance;

            if (Mathf.Abs(remainingDistance) > _detectThreshold)
            {
                vel = Vector3.up * (remainingDistance / _stepSmooth);
            }
        }

        return vel;
    }

    private void ApplyVelocity(Vector3 velocity)
    {
        _rigidbody.linearVelocity = velocity + new Vector3(0f, _yAxisVelocity, 0f);
    }
}
