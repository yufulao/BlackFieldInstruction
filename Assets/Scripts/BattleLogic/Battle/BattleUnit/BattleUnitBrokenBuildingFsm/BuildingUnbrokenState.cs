using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnbrokenState : IFsmComponentState<BattleUnitBrokenBuilding>
{
    public void OnEnter(BattleUnitBrokenBuilding owner)
    {
        owner.BuildingUnbrokenStateEnter();
    }

    public void OnUpdate(BattleUnitBrokenBuilding owner)
    {
        
    }

    public void OnExit(BattleUnitBrokenBuilding owner)
    {
        
    }
}
