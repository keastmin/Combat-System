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
    [SerializeField] private bool _enableGravity = true; // 중력 활성화 여부
    [SerializeField] private Vector3 _gravityAccel = Vector3.down * 9.81f; // 중력 가속도
    [SerializeField] private float _gravitySpeedMax = 20f; // 최대 중력 속도

    [Header("계단")]
    [Tooltip("지면의 높이가 급격하게 바뀌면 계단 스무딩에 해당 값만큼 딜레이를 줍니다." +
             " 지면의 높이가 급격히 바뀌는 기준: 0.1f * (_stepHeight * 2f) 이상의 차이가 있을 때")]
    [SerializeField][Min(0f)] private float _stepSmoothDelay = 0.1f;
    [SerializeField][Min(0f)] private float _stepHeight = 0.3f; // 건널 수 있는 계단 높이
    [Tooltip("값이 클수록 계단 스무딩이 더 부드럽게 됩니다.")]
    [SerializeField][Min(1f)] private float _stepSmooth = 10f; // 계단 스무딩 정도
    [Tooltip("움직이는 동안 계단 스무딩 부드럽게 동작하기 위한 멀티플러")]
    [SerializeField][Min(0f)] private float _stepSmoothMovingMultipler = 1f; // 움직일 때의 스무딩 정도
    [Tooltip("true라면 이동속도를 지면 경사에 맞춥니다")]
    [SerializeField] private bool _alignVelocityToSlope = true; // 이동속도를 지면 경사에 맞추는지 여부
    [Tooltip("계단 스무딩을 위해 앞뒤로 기울기를 감지할 범위를 설정합니다.")]
    [SerializeField][Min(0f)] private float _slopeApproxRange = 1f;
    [SerializeField][Min(0)] private int _slopeApproxIters = 4; // 기울기 근사화 반복 횟수

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
    [Tooltip("지면의 기울기 근사를 위한 디버그 기즈모를 표시합니다.")]
    [SerializeField] private bool _debugSlopeApproximation = false; // 기울기 근사 디버그

    #endregion

    #region 필드

    [SerializeField][HideInInspector] private Rigidbody _rigidbody;
    [SerializeField][HideInInspector] private CapsuleCollider _collider;

    // 상태
    private GroundInfo _collisionGroundInfo = GroundInfo.Empty;
    private bool _collisionIsTouchingCeiling = false;
    private bool _collisionIsTouchingWall = false;
    private Vector3 _wallNormal = Vector3.forward;
    private GroundInfo _groundInfo = GroundInfo.Empty;
    private bool _isOnGround;
    private bool _isOnGroundChangedThisFrame;
    private bool _shouldLeaveGround = false; // 지면을 떠나야 하는지 여부
    private bool _isTouchingCeiling = false; // 천장에 닿았는지 여부
    private Collider _groundCollider = null; // 지면의 콜라이더
    private Rigidbody _groundRb = null; // 지면의 Rigidbody
    private Vector3 _lastNonZeroDirection = Vector3.forward; // 마지막으로 입력된 방향(0이 아닌 벡터)

    // 스텝 스무딩
    private Vector3 _slopePoint = Vector3.zero;
    private Vector3 _slopeNormal = Vector3.up;
    private float _stepSmoothDelayCounter = 0f;

    // 속도
    private Vector3 _velocityGroundRb = Vector3.zero; // 지면에 있는 Rigidbody의 속도
    private Vector3 _velocityGravity = Vector3.zero; // 중력 속도
    private Vector3 _velocityHover = Vector3.zero; // 호버링 속도
    private Vector3 _velocityLeaveGround = Vector3.zero; // 지면을 떠나는 속도
    private Vector3 _velocityInput = Vector3.zero; // 입력 속도

    #endregion

    #region 프로퍼티

    // 중력 여부
    public bool EnableGravity { get => _enableGravity; set => _enableGravity = value; }

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
        _lastNonZeroDirection = _collider.transform.forward;
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
        if (_velocityInput.magnitude > 0f) _lastNonZeroDirection = _velocityInput.normalized; // 입력이 있다면 크기가 0이 아닌 마지막 이동 방향 갱신

        // 현재 지면 위에 존재한다면
        if (IsOnGround)
        {
            _velocityGroundRb = _groundRb == null ? Vector3.zero : _groundRb.linearVelocity; // 지면의 Rigidbody가 있다면 그 속도를 가져옴

            // 입력이 있다면 이동 방향에 대한 기울기의 근사값을 구함
            if(_velocityInput != Vector3.zero)
            {
                _slopeNormal = ApproximateSlope(in _groundInfo, out _slopePoint, 
                    forward: _lastNonZeroDirection, range: _slopeApproxRange, iters: _slopeApproxIters, 
                    debug: _debugSlopeApproximation);
            }

            // 호버 속도 업데이트
            _velocityHover = UpdateStepHoverVelocity(GroundInfo.Distance, deltaTime);
            _velocityGravity = Vector3.zero; // 중력 속도 초기화
        }
        else
        {
            _slopeNormal = Vector3.up; // 입력이 없다면 경사 법선을 위쪽으로 설정
            if (_enableGravity)
            {
                _velocityGravity += _gravityAccel * deltaTime; // 중력 가속도를 적용
                _velocityGravity = Vector3.ClampMagnitude(_velocityGravity, _gravitySpeedMax); // 중력 속도를 최대 속도로 제한
            }
        }

        // 적용할 속도를 조합
        Vector3 velocityGravity = IsLeavingGround || !_enableGravity ? Vector3.zero : _velocityGravity; // 지면으로부터 떠났거나 중력 사용을 안 한다면 중력은 0으로 설정
        // 이거 무슨 동작?
        float alignVelocityToPlaneFactor = _collisionIsTouchingWall ?
            1f - Mathf.Abs(Vector3.Dot(Vector3.ProjectOnPlane(_velocityInput.normalized, Vector3.up), Vector3.ProjectOnPlane(_wallNormal, Vector3.up))) :
            1f;
        Vector3 velocityMove = _alignVelocityToSlope ? AlignVelocityToPlane(_velocityInput, _slopeNormal, alignVelocityToPlaneFactor) : _velocityInput;
        Vector3 velocityToApply = _velocityGroundRb + velocityGravity + _velocityHover + velocityMove + _velocityLeaveGround;
        _velocityLeaveGround = Vector3.MoveTowards(_velocityLeaveGround, Vector3.zero, _rigidbody.mass * deltaTime); // 지면을 떠나는 속도를 질량에 따라 점차 0으로 감소시킴

        ApplyVelocity(velocityToApply); // 최종 속도 적용
    }

    // 원하는 지면 거리를 유지하기 위해 필요한 조정 호버 속도를 계산합니다., 아래의 절차도 잘 이해가 안됌
    private Vector3 UpdateStepHoverVelocity(float groundDistance, float deltaTime, float groundDistanceOffset = 0f, bool smoothing = true)
    {
        Vector3 vel = Vector3.zero;
        float hoverHeightPatch = GroundDistanceDesired + groundDistanceOffset - groundDistance;
        if (_isOnGroundChangedThisFrame || !smoothing)
        {
            vel = Vector3.up * (hoverHeightPatch / deltaTime);
        }
        else
        {
            if(_stepSmoothDelayCounter >= 0f)
            {
                return Vector3.zero; // 아직 스무딩 딜레이가 남아있다면 호버 속도는 0
            }
            float stepSmooth = _stepSmooth;
            if (_velocityInput != Vector3.zero)
            {
                stepSmooth = stepSmooth * _stepSmoothMovingMultipler; // 움직이는 동안 스무딩을 더 부드럽게 하기 위한 멀티플라이어 적용
            }
            float directionSign = Mathf.Sign(hoverHeightPatch);
            float hoverHeightDelta = Mathf.Abs(hoverHeightPatch) / (stepSmooth * deltaTime);
            vel = Vector3.up * directionSign * hoverHeightDelta;
        }

        return vel;
    }

    private void ApplyVelocity(Vector3 velocity)
    {
        _rigidbody.linearVelocity = velocity;
    }

    private void UpdateCleanup()
    {
        _velocityHover = Vector3.zero;
        _velocityInput = Vector3.zero;
        _isOnGroundChangedThisFrame = false;
        _collisionGroundInfo.IsOnGround = false;
        _collisionIsTouchingCeiling = false;
        _collisionIsTouchingWall = false;
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

    // 전방과 후방의 지면의 경사에 대한 법선을 근사화하는 함수
    private Vector3 ApproximateSlope(in GroundInfo groundInfo, out Vector3 slopePoint, Vector3 forward, float range, int iters = 1, bool debug = false)
    {
        // 초기화
        Vector3 origin = GroundProbeOrigin;
        Vector3 up = Vector3.up;
        float maxGroundDistance = GroundDistanceThreshold + 1f;
        float maxHeightDiff = _stepHeight;
        Vector3 slopeNormal = groundInfo.Normal;
        slopePoint = groundInfo.Point;
        float rangeStep = range / (float)iters;

        // 움직임 주체의 앞 방향에서 경사를 감지할 대표 위치를 구함
        Vector3 frontGroundPoint = groundInfo.Point;
        bool frontProxyHit = SampleFarthestGroundPoint(ref frontGroundPoint, maxGroundDistance, maxGroundDistance, forward, rangeStep, iters, debug);

        // 움직임 주체의 뒷 방향에서 경사를 감지할 대표 위치를 구함
        Vector3 backGroundPoint = groundInfo.Point;
        bool backProxyHit = SampleFarthestGroundPoint(ref backGroundPoint, maxGroundDistance, maxGroundDistance, -forward, rangeStep, iters, debug);

        // 2개의 대표 위치로부터 경사 법선을 계산
        if(frontProxyHit || backProxyHit)
        {
            Vector3 slopeSegment = frontGroundPoint - backGroundPoint;
            slopeNormal = Vector3.Cross(slopeSegment, Vector3.Cross(up, slopeSegment)).normalized;

#if UNITY_EDITOR
            if (debug)
            {
                Debug.DrawLine(frontGroundPoint, backGroundPoint, Color.yellow);
            }
#endif
            // 둘 다 지면을 감지한 경우, 여기는 무슨 동작?
            if(frontProxyHit && backProxyHit)
            {
                Vector3 groundProbeSegment = (maxGroundDistance + 100f) * -up;
                bool hasIntersection = GetIntersection(origin, groundProbeSegment, backGroundPoint, slopeSegment, out slopePoint);
            }
        }

        return slopeNormal;
    }

    /// <summary>
    /// 기준점에서 특정 방향으로 가장 멀리 있는 지면 위치를 찾습니다.
    /// <para>각 반복마다, 지정된 방향으로 일정 거리만큼 이동한 후
    /// 해당 위치에서 아래 방향으로 지면을 탐색합니다.</para>
    /// <para>지면 탐색에 성공하면 그 지점을 현재 위치로 갱신하고 계속 진행하며,
    /// 탐색에 실패하면 바로 false를 반환합니다.</para>
    /// </summary>
    /// <param name="groundPoint"></param>
    /// <param name="groundProbeDistance"></param>
    /// <param name="maxHeightDiff"></param>
    /// <param name="direction"></param>
    /// <param name="step"></param>
    /// <param name="iters"></param>
    /// <param name="debug"></param>
    /// <returns></returns>
    private bool SampleFarthestGroundPoint(ref Vector3 groundPoint, float groundProbeDistance, float maxHeightDiff, Vector3 direction, float step, int iters, bool debug = false)
    {
        // 초기화
        Vector3 origin = GroundProbeOrigin;
        Vector3 up = Vector3.up;
        Vector3 prevGroundPoint = groundPoint;
        bool proxyHit = false;

        for(int i = 1; i <= iters; i++)
        {
            Vector3 proxyOrigin = origin + (step * i * direction);
            bool hit = Physics.Raycast(proxyOrigin, -up, out RaycastHit hitInfo,
                maxDistance: groundProbeDistance, layerMask: _groundLayerMask);
#if UNITY_EDITOR
            if (debug)
            {
                Vector3 endHit = proxyOrigin + hitInfo.distance * -up;
                Vector3 endTotal = proxyOrigin + groundProbeDistance * -up;
                Debug.DrawLine(proxyOrigin, endHit, Color.green);
                Debug.DrawLine(endHit, endTotal, Color.grey);
            }
#endif
            if (hit)
            {
                float heightDiff = Vector3.Distance(hitInfo.point, prevGroundPoint);
                proxyHit = true;
                if (heightDiff > maxHeightDiff)
                {
                    return proxyHit;
                }
                groundPoint = hitInfo.point;
                prevGroundPoint = hitInfo.point;
            }
            else return proxyHit;
        }
        return proxyHit;
    }

    // 이건 무슨 동작?
    public static bool GetIntersection(Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2, out Vector3 intersection)
    {
        intersection = Vector3.zero;

        Vector3 cross_v1v2 = Vector3.Cross(v1, v2);
        float denominator = cross_v1v2.sqrMagnitude;

        // 만약 분모가 거의 0이라면, 두 선은 평행하거나 일치합니다. - 이게 무슨 뜻?
        if (denominator < Mathf.Epsilon)
        {
            return false;
        }

        Vector3 p2_p1 = p2 - p1;
        float t = Vector3.Dot(Vector3.Cross(p2_p1, v2), cross_v1v2) / denominator;

        intersection = p1 + t * v1;
        return true;
    }

    #endregion

    #region API

    public void Move(Vector3 velocity)
    {
        _velocityInput = velocity;
    }

    public void LeaveGround()
    {
        if (_isTouchingCeiling) return;
        _shouldLeaveGround = true;
    }

    public void EndLeaveGround()
    {
        _shouldLeaveGround = false;
        _isOnGroundChangedThisFrame = true;
        _velocityGravity = Vector3.zero;
    }

    #endregion

    #region 헬퍼 함수

    // 속도를 지면의 법선에 맞추는 함수
    private Vector3 AlignVelocityToPlane(Vector3 velocity, Vector3 normal, float ratio = 1f)
    {
        float speed = velocity.magnitude;
        Vector3 direction = velocity / speed;
        Vector3 alignedDirection = Quaternion.FromToRotation(Vector3.up, normal) * direction;
        alignedDirection = Vector3.Lerp(direction, alignedDirection, ratio); // 여기서 Lerp는 왜 사용하는가?
        return speed * alignedDirection.normalized;
    }

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
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

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
