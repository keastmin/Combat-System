using UnityEngine;

[RequireComponent(typeof(InputController), typeof(PlayerMover))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _jogSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;

    // move
    private int _moveLevel = 1; // 0: walk, 1: jog, 2: run
    
    // velocity
    private float _targetSpeed;
    private float _currentSpeed;

    // rotation
    private Transform _cameraTransform;
    private float _rotationSpeed = 10f;

    // properties
    public float TargetSpeed { get => _targetSpeed; set => _targetSpeed = value; }
    public float CurrentSpeed { get => _currentSpeed; set => _currentSpeed = value; }
    public int MoveLevel { get => _moveLevel; set => _moveLevel = value; }

    private InputController _inputController;
    private PlayerMover _playerMover;
    private Animator _playerAnimator;

    public InputController InputController => _inputController;
    public PlayerMover PlayerMover => _playerMover;
    public Animator PlayerAnimator => _playerAnimator;

    private void Awake()
    {
        TryGetComponent(out _inputController);
        TryGetComponent(out _playerMover);
        TryGetComponent(out _playerAnimator);

        _cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        
    }

    void Update()
    {
        SetMoveLevel();
    }

    private void FixedUpdate()
    {
        SetSpeed();
        LerpSpeed(Time.fixedDeltaTime);
        RotationBasedCamera(Time.deltaTime);
        PlayerMover.Move(transform.forward * _currentSpeed);
        _playerAnimator.SetFloat("Speed", CurrentSpeed);
    }

    #region Rotation

    private void RotationBasedCamera(float delta)
    {
        if (InputController.MoveInput.magnitude > 0 && _cameraTransform != null)
        {
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;
            forward.y = right.y = 0;

            Vector3 direction = forward * InputController.MoveInput.y + right * InputController.MoveInput.x;

            Quaternion cameraRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, delta * _rotationSpeed);
        }
    }

    #endregion
    
    #region Movement

    private void SetMoveLevel()
    {
        if (InputController.WalkInput)
        {
            _moveLevel = (_moveLevel > 0) ? _moveLevel - 1 : _moveLevel + 1;
        }
        else if (InputController.RunInput)
        {
            _moveLevel = (_moveLevel < 2) ? _moveLevel + 1 : _moveLevel - 1;
        }
    }

    private void SetSpeed()
    {
        if(InputController.MoveInput.magnitude > 0)
        {
            switch (_moveLevel)
            {
                case 0:
                    _targetSpeed = _walkSpeed;
                    break;
                case 1:
                    _targetSpeed = _jogSpeed;
                    break;
                case 2:
                    _targetSpeed = _runSpeed;
                    break;
            }
        }
        else
        {
            _targetSpeed = 0;
            if (_moveLevel == 2) _moveLevel = 1;
        }
    }

    private void LerpSpeed(float delta)
    {
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, delta * 8f);
    }

    #endregion
}
