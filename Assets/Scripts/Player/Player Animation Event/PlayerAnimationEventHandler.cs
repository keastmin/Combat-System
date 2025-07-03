using UnityEngine;

public class PlayerAnimationEventHandler : MonoBehaviour
{
    #region 필드

    // Attack
    private bool _openNextComboInput = false;

    #endregion

    #region 프로퍼티

    // Attack
    public bool OpenNextComboInput => _openNextComboInput;

    #endregion

    #region Attack 애니메이션 이벤트

    public void NextComboAttackOpen()
    {
        _openNextComboInput = true;
    }

    public void NextComboAttackClose()
    {
        _openNextComboInput = false;
    }

    #endregion
}
