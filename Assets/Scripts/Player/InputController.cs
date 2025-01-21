using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    private Vector2 _moveInput;

    public Vector2 MoveInput => _moveInput;

    PlayerInput _input;

    InputAction _moveAction;

    private void Awake()
    {
        TryGetComponent(out PlayerInput input);
        this._input = input;
        _moveAction = input.actions.FindAction("Move");
    }

    void Update()
    {
        MoveInputDetect();
    }

    private void MoveInputDetect()
    {
        if(_moveAction != null)
        {
            _moveInput = _moveAction.ReadValue<Vector2>();
        }
    }
}
