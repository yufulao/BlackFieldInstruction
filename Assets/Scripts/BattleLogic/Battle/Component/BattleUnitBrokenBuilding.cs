using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitBrokenBuilding : BattleUnit
{
    private FsmComponent<BattleUnitBrokenBuilding> _fsm;

    private void Update()
    {
        _fsm.OnUpdate();
    }

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        _fsm = new FsmComponent<BattleUnitBrokenBuilding>(this);
        InitFsm(); //挂起状态
        _fsm.ChangeFsmState(typeof(BuildingNormalState));
    }

    public override void OnUnitReset()
    {
        base.OnUnitReset();
        _fsm.ChangeFsmState(typeof(BuildingNormalState));
    }

    private void InitFsm()
    {
        Dictionary<Type, IFsmComponentState<BattleUnitBrokenBuilding>> fsmStates = new Dictionary<Type, IFsmComponentState<BattleUnitBrokenBuilding>>();
        fsmStates.Add(typeof(BuildingNormalState), new BuildingNormalState());
        fsmStates.Add(typeof(BuildingBrokenState), new BuildingBrokenState());
        fsmStates.Add(typeof(BuildingRuinState), new BuildingRuinState());
        fsmStates.Add(typeof(BuildingCleanState), new BuildingCleanState());
        _fsm.SetFsm(fsmStates);
        //fsm此时处于挂起状态，没有state
    }

    public void BuildingNormalStateEnter()
    {
        
    }
}
