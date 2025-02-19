using UnityEngine;

// This is the API part of the PlayerMover class.
public partial class PlayerMover : MonoBehaviour
{
    public void Move(Vector3 velocity)
    {
        _velocityInput = velocity;
    }

    public void Move(Vector3 direction, float speed)
    {
        _velocityInput = direction * speed;
    }

    public void ClearVelocity()
    {
        InitValiable();
        _velocityInput = Vector3.zero;
        _gravityForce = 0f;
    }

    public void Jump(float jumpForce)
    {
        _yAxisVelocity = jumpForce;
        _gravityForce = 0f;
        _isJumping = true;
    }
}
