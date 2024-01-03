using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitTarget : BattleUnit
{
    [SerializeField] private List<Material> lessThanMaterial=new List<Material>();
    [SerializeField] private List<Material> moreThanMaterial=new List<Material>();
    [SerializeField] private MeshRenderer meshRenderer;
    public int needPeopleCount;

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
    /// 积分是否达标设置target状态
    /// </summary>
    /// <param name="currentPeople"></param>
    public void SetTargetState(int currentPeople)
    {
        if (currentPeople>=needPeopleCount)
        {
            SetMoreThanPeopleCountNeed();
            return;
        }

        SetLessThanPeopleCountNeed();
    }

    private void SetLessThanPeopleCountNeed()
    {
        meshRenderer.SetMaterials(lessThanMaterial);
    }
    
    private void SetMoreThanPeopleCountNeed()
    {
        meshRenderer.SetMaterials(moreThanMaterial);
    }

    private void ResetAll()
    {
        SetLessThanPeopleCountNeed();
    }
}
