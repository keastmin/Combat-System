using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("�ݶ��̴�")]
    [SerializeField] private float _height = 2f; // �ݶ��̴� ����
    [SerializeField] private float _thickness = 1f; // �ݶ��̴� �β�
    [SerializeField] private Vector3 _offset = Vector3.zero; // �ݶ��̴� ������

    [Header("����")]
    [SerializeField] private float _gravityAcc = 9.81f; // �߷� ���ӵ�
    [SerializeField] private float _maxGravitySpeed = 20f; // �ִ� �߷� �ӵ�

    [Header("���")]
    [SerializeField][Min(0f)] private float _stepHeight = 0.3f; // �ǳ� �� �ִ� ��� ����

    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    // �Է�
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

    #region �ھ�

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

    #region �ʱ�ȭ

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
