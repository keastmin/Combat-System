using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public partial class PlayerMover : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField][Min(0)] private float _height = 2.0f;
    [SerializeField][Min(0)] private float _radius = 0.5f;
    [SerializeField] private Vector3 _colliderOffset = Vector3.zero;

    [Header("Step")]
    [SerializeField] [Min(0)] private float _stepHeight = 0.3f;

    [Header("Physics")]
    [SerializeField] private bool _useGravity = true;
    [SerializeField][Min(0)] private float _gravityAcc = 9.81f;
    [SerializeField][Min(0)] private float _maxFallSpeed = 15f;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float _rayDistance = 10f;
    [SerializeField] private float _rayRadius = 0.1f;
    [SerializeField] private float _detectThreshold = 0.01f;

    [Header("Debug")]
    [SerializeField] private bool _groundDetectDebug = true;

    // Velocity
    private Vector3 _velocityInput = Vector3.zero;
    private float _yAxisVelocity = 0f;
    private float _gravityForce = 0f;

    // Ground
    GroundInfo _groundInfo = GroundInfo.Empty;
    bool _isGround = false;

    // Cache
    private float _colliderHalfHeight;

    // Properties
    public bool IsGround => _isGround;

    Rigidbody _rigidbody;
    CapsuleCollider _capsuleCollider;

    private void OnValidate()
    {
        InitComponents();
        InitColliderProperties();

        _colliderHalfHeight = _capsuleCollider.height / 2f;
    }

    private void Awake()
    {
        OnValidate();
    }

    private void FixedUpdate()
    {
        MoveProcess();
    }

    #region Init

    // Set Rigidbody and CapsuleCollider components
    private void InitComponents()
    {
        TryGetComponent(out _rigidbody);
        _rigidbody.useGravity = false;
        _rigidbody.freezeRotation = true;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        TryGetComponent(out _capsuleCollider);
    }

    // Set CapsuleCollider properties
    private void InitColliderProperties()
    {
        ColliderUtill.SetHeight(_capsuleCollider, _height, _stepHeight, _colliderOffset);
        ColliderUtill.SetRadius(_capsuleCollider, _radius);
    }

    #endregion

    #region Gizmo Debug

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawRay()
    }

    #endregion
}
