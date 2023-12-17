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
            CheckCellAfterActive();
        }
    }

    public void SetFireActive(bool active)
    {
        if (currentActive!=active)
        {
            currentActive = active;
            SetVfxActive(active);
        }
    }

    private void ResetAll()
    {
        SetVfxActive(originalActive);
    }

    private void SetVfxActive(bool active)
    {
        if (active)
        {
            fireVfx.Play();
            return;
        }
        fireVfx.Stop();
    }
    
    private void CheckCellAfterActive()
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