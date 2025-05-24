using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("콜라이더")]
    [SerializeField] private float _height = 2f; // 콜라이더 높이
    [SerializeField] private float _thickness = 1f; // 콜라이더 두께
    [SerializeField] private Vector3 _offset = Vector3.zero; // 콜라이더 오프셋

    [Header("물리")]
    [SerializeField] private float _gravityAcc = 9.81f; // 중력 가속도
    [SerializeField] private float _maxGravitySpeed = 20f; // 최대 중력 속도

    [Header("계단")]
    [SerializeField][Min(0f)] private float _stepHeight = 0.3f; // 건널 수 있는 계단 높이

    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    // 입력
    private Vector3 _inputVelocity;

    private void OnValidate()
    {
        InitComponents();
        SetColliderDimention();
    }

    private void Awake()
    {
        OnValidate();
    }

    private void FixedUpdate()
    {
        MoveVelocity(_inputVelocity);
        InitValue();
    }

    #region 코어

    private void MoveVelocity(Vector3 velocity)
    {
        _rigidbody.linearVelocity = velocity;
    }

    private void InitValue()
    {
        _inputVelocity = Vector3.zero;
    }

    #endregion

    #region API

    public void Move(Vector3 velocity)
    {
        _inputVelocity = velocity;
    }

    #endregion

    #region 초기화

    private void InitComponents()
    {
        TryGetComponent(out _rigidbody);
        _rigidbody.useGravity = false;
        _rigidbody.freezeRotation = true;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        TryGetComponent(out _capsuleCollider);
    }

    private void SetColliderDimention()
    {
        SetColliderHeight();
        SetColliderRadius();
    }

    private void SetColliderHeight()
    {
        if (_stepHeight > _height) _stepHeight = _height;
        float centerY = (_height + _stepHeight) / 2f;
        Vector3 center = _offset + new Vector3(0f, centerY, 0f);
        _capsuleCollider.height = _height - _stepHeight;
        _capsuleCollider.center = center;
        LimitColliderValue();
    }

    private void SetColliderRadius()
    {
        float radius = _thickness / 2f;
        _capsuleCollider.radius = radius;
        LimitColliderValue();
    }

    private void LimitColliderValue()
    {
        if (_capsuleCollider.height < _capsuleCollider.radius * 2f) _capsuleCollider.radius = _capsuleCollider.height / 2f;
    }

    #endregion
}
