using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitFire : BattleUnit
{
    [SerializeField] private bool originalActive; //初始是否在燃烧
    [SerializeField] public ParticleSystem fireVfx; //初始是否在燃烧
    [HideInInspector] public bool currentActive; //当前是否在燃烧

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        currentActive = originalActive;
        ResetAll();
    }

    public override void OnUnitReset()
    {
        base.OnUnitReset();
        ResetAll();
    }

    public override IEnumerator CheckOverlap()
    {
        yield  return base.CheckOverlap();
        if (currentActive)
        {
            CheckUnit();
        }
    }

    /// <summary>
    /// 设置火的状态
    /// </summary>
    /// <param name="active"></param>
    public void SetFireActive(bool active)
    {
        if (currentActive!=active)
        {
            currentActive = active;
            SetVfxActive(active);
        }
    }

    /// <summary>
    /// 重置unit
    /// </summary>
    private void ResetAll()
    {
        SetFireActive(originalActive);
    }

    /// <summary>
    /// 开启或关闭火的特效
    /// </summary>
    /// <param name="active"></param>
    private void SetVfxActive(bool active)
    {
        if (active)
        {
            fireVfx.Play();
            return;
        }
        fireVfx.Stop();
    }
    
    /// <summary>
    /// 检测特定unit
    /// </summary>
    private void CheckUnit()
    {
        BattleManager.Instance.CheckCellForUnit<BattleUnitPeople>(this, UnitType.People, (people) =>
        {
            for (int i = 0; i < people.Count; i++)
            {
                people[i].SetPeopleActive(false);
            }
        });
    }
}