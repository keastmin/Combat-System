using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    private Vector2 _moveInput = Vector2.zero;
    private bool _runInput = false;
    private bool _walkInput = false;
    private bool _dodgeInput = false;
    private bool _jumpInput = false;

    public Vector2 MoveInput => _moveInput;
    public bool RunInput => _runInput;
    public bool WalkInput => _walkInput;
    public bool DodgeInput => _dodgeInput;
    public bool JumpInput => _jumpInput;

    PlayerInput _input;

    InputAction _moveAction;
    InputAction _runAction;
    InputAction _walkAction;
    InputAction _dodgeAction;
    InputAction _jumpAction;

    private void Awake()
    {
        TryGetComponent(out _input);
        _moveAction = _input.actions.FindAction("Move");
        _runAction = _input.actions.FindAction("Run");
        _walkAction = _input.actions.FindAction("Walk");
        _dodgeAction = _input.actions.FindAction("Dodge");
        _jumpAction = _input.actions.FindAction("Jump");
    }

    void Update()
    {
        MoveInputDetect();
        RunInputDetect();
        WalkInputDetect();
        DodgeInputDetect();
        JumpInputDetect();
    }

    private void MoveInputDetect()
    {
        if(_moveAction != null)
        {
            _moveInput = _moveAction.ReadValue<Vector2>();
        }
    }

    private void RunInputDetect()
    {
        if(_runAction != null)
        {
            _runInput = _runAction.triggered;
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

    private void DodgeInputDetect()
    {
        if(_dodgeAction != null)
        {
            _dodgeInput = _dodgeAction.triggered;
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
