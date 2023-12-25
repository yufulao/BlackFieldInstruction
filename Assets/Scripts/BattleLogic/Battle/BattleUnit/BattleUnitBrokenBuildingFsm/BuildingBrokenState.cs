using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingBrokenState : IFsmComponentState<BattleUnitBrokenBuilding>
{
    public void OnEnter(BattleUnitBrokenBuilding owner)
    {
        owner.BuildingBrokenStateEnter();
    }

    public void OnUpdate(BattleUnitBrokenBuilding owner)
    {
        
    }

    public void OnExit(BattleUnitBrokenBuilding owner)
    {
        
    }
}
