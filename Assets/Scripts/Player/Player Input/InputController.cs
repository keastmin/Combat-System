using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    [Header("더블탭")]
    [SerializeField] private float _doubleTabTime = 0.3f;

    [Header("달리기 중 턴")]
    [SerializeField] private float _turnInputTime = 0.1f; // 이동 반대 방향 입력 시간

    // Player Input 컴포넌트
    PlayerInput _input;

    // 내부 사용 변수
    private bool[] _lastInput = new bool[4]; // W, A, S, D 입력 상태
    private float[] _lastInputTime = new float[4]; // 마지막 입력 시간

    // 외부 사용 변수
    private Vector2 _moveInput = Vector2.zero; // WASD
    private bool _walkInput = false; // Left Ctrl
    private bool _jumpInput = false; // Space
    private bool _dodgeInput = false; // WASD Double Tab

    // 프로퍼티
    public Vector3 MoveInput => new Vector3(_moveInput.x, 0f, _moveInput.y);
    public bool WalkInput => _walkInput;
    public bool JumpInput => _jumpInput;
    public bool DodgeInput => _dodgeInput;

    InputAction _moveAction;
    InputAction _walkAction;
    InputAction _jumpAction;
    InputAction _dodgeAction;

    private void Awake()
    {
        TryGetComponent(out _input);
        _moveAction = _input.actions.FindAction("Move");
        _walkAction = _input.actions.FindAction("Walk");
        _jumpAction = _input.actions.FindAction("Jump");
        _dodgeAction = _input.actions.FindAction("Dodge");
    }

    void Update()
    {
        MoveInputDetect();
        WalkInputDetect();
        JumpInputDetect();
        DodgeInputDetect();
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

    private void DodgeInputDetect()
    {
        _dodgeInput = false; // 초기화

        // 입력 받기, 처음 누르는 그 한 순간만 대응되는 lastInput[i]에 대응, W는 0, A는 1, S는 2, D는 3
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetDodgeInput(0);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetDodgeInput(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetDodgeInput(2);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetDodgeInput(3);
        }

        // 마지막 버튼 누름으로부터 시간 체크
        DodgeTimeCheck();
    }

    private void SetDodgeInput(int index)
    {
        if (!_lastInput[index])
        {
            _lastInput[index] = true; // 해당 인덱스 입력 true
        }
        else
        {
            _dodgeInput = true; // 더블 탭 입력 발생

            for(int i = 0; i < 4; i++)
            {
                _lastInputTime[i] = 0f; // 시간 초기화
                _lastInput[i] = false; // 입력 상태 초기화
            }
        }
    }

    // 각 인덱스 시간 체크
    private void DodgeTimeCheck()
    {       
        for (int i = 0; i < 4; i++)
        {
            if (_lastInput[i])
            {
                _lastInputTime[i] += Time.deltaTime;

                if (_lastInputTime[i] > _doubleTabTime)
                {
                    _lastInputTime[i] = 0f; // 시간 초기화
                    _lastInput[i] = false; // 입력 상태 초기화
                }
            }
        }
    }
}
