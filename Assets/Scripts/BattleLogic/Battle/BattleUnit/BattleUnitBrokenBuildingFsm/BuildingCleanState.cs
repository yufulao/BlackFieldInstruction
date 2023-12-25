using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCleanState : IFsmComponentState<BattleUnitBrokenBuilding>
{
    public void OnEnter(BattleUnitBrokenBuilding owner)
    {
        owner.BuildingCleanStateEnter();
    }

    public void OnUpdate(BattleUnitBrokenBuilding owner)
    {
        
    }

    public void OnExit(BattleUnitBrokenBuilding owner)
    {
        
    }
}
