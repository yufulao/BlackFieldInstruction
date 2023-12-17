using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitRuin : BattleUnit
{
    [HideInInspector] public bool currentActive; //当前是否在燃烧
    [SerializeField] private ParticleSystem ruinVfx; //初始是否在燃烧

    public override void OnUnitReset()
    {
        base.OnUnitReset();
        gameObject.SetActive(currentActive);
        SetVfxActive(currentActive);
    }

    public override IEnumerator CheckOverlap()
    {
        yield return base.CheckOverlap();
        if (currentActive)
        {
            CheckCellAfterActive();
        }
    }

    public void SetRuinActive(bool active)
    {
        currentActive = active;
        walkable = !active;
        gameObject.SetActive(active);
        SetVfxActive(active);
    }

    private void CheckCellAfterActive()
    {
        BattleManager.Instance.CheckCellForUnit<BattleUnitFire>(this, UnitType.Fire, (fires) =>
        {
            for (int i = 0; i < fires.Count; i++)
            {
                fires[i].SetFireActive(false);
            }
        });
        BattleManager.Instance.CheckCellForUnit<BattleUnitPeople>(this, UnitType.People, (people) =>
        {
            for (int i = 0; i < people.Count; i++)
            {
                people[i].SetPeopleActive(false);
            }
        });
    }

    private void SetVfxActive(bool active)
    {
        if (active)
        {
            ruinVfx.Play();
            return;
        }
        ruinVfx.Stop();
    }
}
