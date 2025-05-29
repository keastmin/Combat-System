using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct GroundInfo
{
    public static GroundInfo Empty => new GroundInfo(false, 0f, Vector3.zero, Vector3.up, null);
    public bool IsOnGround;
    public float Distance;
    public Vector3 Point;
    public Vector3 Normal;
    public Collider Collider;

    public GroundInfo(bool isOnGround, float distance, Vector3 point, Vector3 normal, Collider collider)
    {
        IsOnGround = isOnGround;
        Distance = distance;
        Point = point;
        Normal = normal;
        Collider = collider;
    }
}

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    #region 변수

    #region 인스펙터 필드

    [Header("콜라이더")]
    [SerializeField] private float _height = 2f; // 콜라이더 높이
    [SerializeField] private float _thickness = 1f; // 콜라이더 두께
    [SerializeField] private Vector3 _offset = Vector3.zero; // 콜라이더 오프셋

    [Header("물리")]
    [SerializeField] private float _gravityAcc = 9.81f; // 중력 가속도
    [SerializeField] private float _maxGravitySpeed = 20f; // 최대 중력 속도

    [Header("계단")]
    [Tooltip("지면의 높이가 급격하게 바뀌면 계단 스무딩에 해당 값만큼 딜레이를 줍니다." +
             " 지면의 높이가 급격히 바뀌는 기준: 0.1f * (_stepHeight * 2f) 이상의 차이가 있을 때")]
    [SerializeField][Min(0f)] private float _stepSmoothDelay = 0.1f;
    [SerializeField][Min(0f)] private float _stepHeight = 0.3f; // 건널 수 있는 계단 높이

    [Header("지면 검사")]
    [Tooltip("지면 레이어 정의")]
    [SerializeField] private LayerMask _groundLayerMask = 1 << 0;
    [SerializeField][Min(0f)] private float _groundProbeExtraDistance = 10f;
    [SerializeField][Min(0f)] private float _groundProbeThickness = 0.1f;
    [Tooltip("true라면 지면 검사의 두께가 0보다 클 때, 실제 지면 법선을 찾기 위해 2차 검사 Ray를 쏩니다.")]
    [SerializeField] private bool _groundProbeFindRealNormal = false;

    [Header("디버그")]
    [Tooltip("지면 감지를 위한 디버그 기즈모를 표시합니다.")]
    [SerializeField] private bool _debugGroundDetection = false; // 지면 검사 디버그

    #endregion

    #region 필드

    [SerializeField][HideInInspector] private Rigidbody _rigidbody;
    [SerializeField][HideInInspector] private CapsuleCollider _collider;

    // 상태
    private GroundInfo _collisionGroundInfo = GroundInfo.Empty;
    private bool _hasDirectCollision = false; 
    private bool _collisionIsTouchingCeiling = false;
    private bool _collisionIsTouchingWall = false;
    private Vector3 _wallNormal = Vector3.forward;
    private GroundInfo _groundInfo = GroundInfo.Empty;
    private bool _isOnGround;
    private bool _isOnGroundChangedThisFrame;
    private bool _shouldLeaveGround = false; // 지면을 떠나야 하는지 여부
    private bool _isTouchingCeiling = false; // 천장에 닿았는지 여부
    private Collider _groundCollider = null;
    private Vector3 _lastNonZeroDirection = Vector3.forward; // 마지막으로 입력된 방향(0이 아닌 벡터)

    // 스텝 스무딩
    private float _stepSmoothDelayCounter = 0f;

    // 속도
    private Vector3 _velocityGravity; // 중력 속도
    private Vector3 _velocityLeaveGround = Vector3.zero; // 지면을 떠나는 속도
    private Vector3 _velocityInput = Vector3.zero; // 입력 속도

    #endregion

    #region 프로퍼티

    // 지면 감지 정보
    public GroundInfo GroundInfo
    {
        get => _groundInfo;
        private set
        {
            _groundInfo = value;
            IsOnGround = value.IsOnGround;
            GroundCollider = value.IsOnGround ? value.Collider : null;
        }
    }
    // 이번 물리 프레임에 이동하는 주체가 지면 위에 있는지 여부
    public bool IsOnGround
    {
        get => _isOnGround;
        private set
        {
            if(value != _isOnGround)
            {
                HandleIsOnGroundChange(value);
            }
            _isOnGround = value;
        }
    }
    // 천장에 닿았는지 여부
    public bool IsTouchingCeiling
    {
        get => _isTouchingCeiling;
        private set
        {
            if(value == true && value != _isTouchingCeiling)
            {
                HandleIsTouchingCeilingChange(value);
            }
            _isTouchingCeiling = value;
        }
    }
    // 이동하는 주체의 아래에 있는 지면의 콜라이더
    public Collider GroundCollider
    {
        get => _groundCollider;
        private set => _groundCollider = value;
    }
    // true라면, 감지된 지면을 무시하고 자신이 지면에 있지 않은 것처럼 강제함
    private bool IsLeavingGround => _shouldLeaveGround || _velocityLeaveGround != Vector3.zero;
    // 캡슐 콜라이더의 절반 높이
    private float ColliderHalfHeight => _collider.height / 2f;
    // 캡슐 콜라이더의 센터
    private Vector3 GroundProbeOrigin => _collider.transform.position + GroundDistanceDesired * Vector3.up;
    private float GroundProbeDistance => GroundDistanceThreshold + _groundProbeExtraDistance;
    private float GroundDistanceThreshold
    {
        get
        {
            float value = GroundDistanceDesired;
            if (IsOnGround) value += _stepHeight;
            return value * 1.01f;
        }
    }
    // 캡슐 콜라이더의 중심으로부터 원하는 지면까지의 거리
    private float GroundDistanceDesired => ColliderHalfHeight + _stepHeight;
    #endregion

    #region 이벤트

    public event Action<bool> OnIsOnGroundChanged = delegate { };
    public event Action<bool> OnIsTouchingCeilingChanged = delegate { };

    #endregion

    #endregion

    #region MonoBehaviour 함수

    private void OnCollisionStay(Collision collision)
    {
        _hasDirectCollision = true;
        // 이 부분은 무슨 동작?
        CheckDirectCollision(collision,
            out _collisionGroundInfo,
            out _collisionIsTouchingCeiling,
            out _collisionIsTouchingWall);
    }

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
        UpdateCollisionCheck();
        UpdateMovement(Time.fixedDeltaTime);
        UpdateCleanup();
    }

    #endregion

    #region 코어

    // 참조가 1개라면 bool로 선언한 이유가?
    private bool CheckDirectCollision(Collision collision, out GroundInfo groundInfo, out bool isTouchingCeiling, out bool isTouchingWall)
    {
        groundInfo = new GroundInfo();
        groundInfo.IsOnGround = false;
        isTouchingCeiling = false;
        isTouchingWall = false;
        // 이거 무슨 동작?
        if(!LayerMaskContains(_groundLayerMask, collision.gameObject.layer))
        {
            return false;
        }

        // ContactPoint는 무엇인가?
        ContactPoint contact = collision.GetContact(0);
        // 이 동작의 의미는 무엇인가?
        if(contact.normal.y > 0.01f)
        {
            groundInfo.Distance = GroundDistanceDesired;
            groundInfo.Point = contact.point;
            groundInfo.Normal = contact.normal;
            groundInfo.IsOnGround = true;
            groundInfo.Collider = collision.collider;
        }
        else if(contact.normal.y < -0.25f)
        {
            isTouchingCeiling = true;
        }
        else
        {
            isTouchingWall = true;
            _wallNormal = contact.normal;
        }

        return true;
    }

    // 콜리젼 업데이트
    private void UpdateCollisionCheck()
    {
        // 지면을 감지하고 지면 정보 업데이트
        GroundInfo newGroundInfo = Probe(findRealNormal: _groundProbeFindRealNormal, debug: _debugGroundDetection);
    
        // 지면까지의 거리가 급격하게 변경되면 계단 스무딩을 잠시 중지
        if(Mathf.Abs(newGroundInfo.Distance - GroundInfo.Distance) > 0.1f * (_stepHeight * 2f))
        {
            _stepSmoothDelayCounter = _stepSmoothDelay;
        }

        // 지면을 떠났을 때
        if (IsLeavingGround)
        {
            // 천장에 닿으면 지면을 떠나는 것을 중지
            if (_isTouchingCeiling) EndLeaveGround();
            else newGroundInfo.IsOnGround = false;
        }

        // 지면 정보 업데이트
        GroundInfo = newGroundInfo;

        // 콜라이더가 천장에 닿았는지 여부
        IsTouchingCeiling = _collisionIsTouchingCeiling;
    }

    // 실제 이동을 처리하는 함수
    private void UpdateMovement(float deltaTime)
    {
        if (_stepSmoothDelayCounter > 0f) _stepSmoothDelayCounter -= deltaTime; // 계단 스무딩 딜레이가 있다면 그만큼 감소
        if (_velocityGravity.magnitude > 0f) _lastNonZeroDirection = _velocityInput.normalized; // 입력이 있다면 크기가 0이 아닌 마지막 이동 방향 갱신

        // 현재 지면 위에 존재한다면
        if (IsOnGround)
        {

        }
    }

    private void UpdateCleanup()
    {

    }

    #endregion

    #region 지면 감지

    // 지면을 감지하고 지면이 감지되면 해당 지면의 정보를 반환하는 함수
    private GroundInfo Probe(bool findRealNormal = false, bool debug = false)
    {
        // 초기화
        Vector3 origin = GroundProbeOrigin;
        Vector3 up = Vector3.up;
        float distance = GroundProbeDistance;
        float thickness = _groundProbeThickness;
        GroundInfo groundInfo = GroundInfo.Empty;
        bool hit = false;
        RaycastHit hitInfo;

        // 지면을 감지하기 위한 Ray의 두께에 따라 Raycast 또는 SphereCast를 사용
        if (thickness <= 0f) hit = Physics.Raycast(origin, -up, out hitInfo, maxDistance: distance, layerMask: _groundLayerMask);
        else hit = Physics.SphereCast(origin, thickness / 2f, -up, out hitInfo, maxDistance: distance, layerMask: _groundLayerMask);

        // 지면이 감지되면 해당 지면의 정보를 저장
        if (hit)
        {
            groundInfo.Distance = Vector3.Distance(hitInfo.point, origin);
            groundInfo.Normal = hitInfo.normal;
            groundInfo.Point = hitInfo.point;
            groundInfo.IsOnGround = (groundInfo.Distance <= GroundDistanceThreshold) && (groundInfo.Normal.y > 0); // 여기서 Normal.y를 검사하는 부분에서 특정 경사도 이하인 경우만 지상으로 간주 가능
            if (groundInfo.IsOnGround) groundInfo.Collider = hitInfo.collider;

            // 지면이 감지되었을 때, 두께가 0보다 크면 실제 지면 법선과 다를 수 있다.
            // 이 경우 실제 지면 법선을 찾기 위해 추가적인 Raycast를 수행
            if(findRealNormal && groundInfo.IsOnGround && thickness > 0f)
            {
                Vector3 tmpOrigin = hitInfo.point + 0.01f * -up;
                if(hitInfo.collider.Raycast(new Ray(tmpOrigin, -up), out RaycastHit realNormalHitInfo, maxDistance: 0.1f))
                {
                    groundInfo.Normal = realNormalHitInfo.normal;
                }
            }
        }

#if UNITY_EDITOR
        if (debug)
        {
            Vector3 end = origin + new Vector3(0f, -distance, 0f);
            Debug.DrawLine(origin, end, Color.grey);
        }
#endif

        return groundInfo;
    }

    #endregion

    #region API

    public void Move(Vector3 velocity)
    {
        _velocityInput = velocity;
    }

    public void EndLeaveGround()
    {
        _shouldLeaveGround = false;
        _isOnGroundChangedThisFrame = true;
        _velocityGravity = Vector3.zero;
    }

    #endregion

    #region 헬퍼 함수

    private bool LayerMaskContains(LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    #endregion

    #region 이벤트

    private void HandleIsOnGroundChange(bool isOnGround)
    {
        _isOnGroundChangedThisFrame = true;
        OnIsOnGroundChanged.Invoke(isOnGround);
        _velocityGravity = Vector3.zero;
    }

    private void HandleIsTouchingCeilingChange(bool isTouchingCeiling)
    {
        OnIsTouchingCeilingChanged.Invoke(isTouchingCeiling);
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

        TryGetComponent(out _collider);
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
        _collider.height = _height - _stepHeight;
        _collider.center = center;
        LimitColliderValue();
    }

    private void SetColliderRadius()
    {
        float radius = _thickness / 2f;
        _collider.radius = radius;
        LimitColliderValue();
    }

    private void LimitColliderValue()
    {
        if (_collider.height < _collider.radius * 2f) _collider.radius = _collider.height / 2f;
    }

    #endregion
}
