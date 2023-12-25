using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitPeople : BattleUnit
{
    [SerializeField] private bool originalActive;
    [SerializeField]private GameObject peopleObj;

    [HideInInspector] public bool currentActive;

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        ResetAll();
    }

    public override void OnUnitReset()
    {
        base.OnUnitReset();
        ResetAll();
    }

    /// <summary>
    /// 设置人群的状态
    /// </summary>
    /// <param name="active"></param>
    public void SetPeopleActive(bool active)
    {
        if (currentActive!=active)
        {
            currentActive = active;
            peopleObj.SetActive(currentActive);
        }
    }

    /// <summary>
    /// 重置unit
    /// </summary>
    private void ResetAll()
    {
        SetPeopleActive(originalActive);
    }
    
}
