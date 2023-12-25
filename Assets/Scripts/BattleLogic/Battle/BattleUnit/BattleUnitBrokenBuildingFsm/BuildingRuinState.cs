using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRuinState : IFsmComponentState<BattleUnitBrokenBuilding>
{
    public void OnEnter(BattleUnitBrokenBuilding owner)
    {
        owner.BuildingRuinStateEnter();
    }

    public void OnUpdate(BattleUnitBrokenBuilding owner)
    {
        
    }

    public void OnExit(BattleUnitBrokenBuilding owner)
    {
        
    }
}
