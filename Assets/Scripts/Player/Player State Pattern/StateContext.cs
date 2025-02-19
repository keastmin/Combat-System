using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateContext
{
    // Attack
    public int MaxAttackCombo;
    public int CurrentAttackCombo;
    public List<ComboAttackData> ComboAttackDatas;

    public StateContext(List<ComboAttackData> comboAttackDatas)
    {
        this.ComboAttackDatas = comboAttackDatas;
        this.MaxAttackCombo = comboAttackDatas.Count;
        this.CurrentAttackCombo = 0;
    }
}
