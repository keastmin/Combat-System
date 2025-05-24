using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    // Player Input 컴포넌트
    PlayerInput _input;

    // 외부 사용 변수
    private Vector2 _moveInput = Vector2.zero; // WASD
    private bool _walkInput = false; // Left Ctrl
    private bool _jumpInput = false; // Space

    // 프로퍼티
    public Vector2 MoveInput => _moveInput;
    public bool WalkInput => _walkInput;
    public bool JumpInput => _jumpInput;

    InputAction _moveAction;
    InputAction _walkAction;
    InputAction _jumpAction;

    private void Awake()
    {
        TryGetComponent(out _input);
        _moveAction = _input.actions.FindAction("Move");
        _walkAction = _input.actions.FindAction("Walk");
        _jumpAction = _input.actions.FindAction("Jump");
    }

    void Update()
    {
        MoveInputDetect();
        WalkInputDetect();
        JumpInputDetect();
    }

    private void MoveInputDetect()
    {
        if(_moveAction != null)
        {
            _moveInput = _moveAction.ReadValue<Vector2>();
        }
    }

    private void WalkInputDetect()
    {
        if(_walkAction != null)
        {
            _walkInput = _walkAction.phase == InputActionPhase.Started ||  
                         _walkAction.phase == InputActionPhase.Performed;
        }
    }

    private void JumpInputDetect()
    {
        if(_jumpAction != null)
        {
            _jumpInput = _jumpAction.triggered;
        }
    }
}
