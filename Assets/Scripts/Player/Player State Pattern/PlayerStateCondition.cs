using System;
using TMPro;
using UnityEngine;

[Serializable]
public class PlayerStateCondition
{
    [SerializeField] private TextMeshProUGUI _stateFlowDebugText;
    [SerializeField] private bool _stateFlowDebug = false;
    [SerializeField][Range(1, 5)] private int _stateFlowLimit;

    // 플레이어 컨트롤러
    private PlayerController _controller;

    // 움직임 입력이 있는지 여부
    public bool MoveInput => _controller.InputC.MoveInput.sqrMagnitude > 0.01f;

    public PlayerStateCondition(PlayerController controller)
    {
        _controller = controller;
    }
}
