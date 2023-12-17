using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitBrokenBuilding : BattleUnit
{
    [SerializeField] private BrokenBuildingStateType originalStateType;
    [SerializeField] private int timeToRuin;
    [SerializeField] private int timeToClean;
    [HideInInspector] public BrokenBuildingStateType currentStateType;
    [SerializeField] private ForwardType forwardForBrokenAtStart;
    [SerializeField] private BattleUnitRuin ruinUnit;
    [SerializeField] private GameObject unBrokenBuilding;
    [SerializeField] private GameObject brokenBuilding;
    
    private FsmComponent<BattleUnitBrokenBuilding> _fsm;
    private ForwardType _cacheForward;
    private int _cacheTimeToRuin;
    private int _cacheTimeToClean;

    private void Update()
    {
        _fsm?.OnUpdate();
    }

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        InitFsm(); //挂起状态
        ResetAll();
    }

    public override void OnUnitReset()
    {
        base.OnUnitReset();
        ResetAll();
    }

    public override IEnumerator Execute()
    {
        yield return base.Execute();
        switch (currentStateType)
        {
            case BrokenBuildingStateType.Broken:
                _cacheTimeToRuin--;
                if (_cacheTimeToRuin<0)
                {
                    ToRuin(_cacheForward);
                }
                break;
            case BrokenBuildingStateType.Ruin:
                _cacheTimeToClean--;
                if (_cacheTimeToClean<0)
                {
                    ToClean();
                }
                break;
        }
    }
    
    public bool GetWalkable()
    {
        return currentStateType == BrokenBuildingStateType.Ruin || currentStateType == BrokenBuildingStateType.Clean;
    }

    public void BuildingUnbrokenStateEnter()
    {
        unBrokenBuilding.SetActive(true);
        brokenBuilding.SetActive(false);
    }

    public void BuildingBrokenStateEnter()
    {
        unBrokenBuilding.SetActive(false);
        brokenBuilding.SetActive(true);
    }

    public void BuildingRuinStateEnter()
    {
        brokenBuilding.SetActive(false);
        ruinUnit.SetRuinActive(true);
    }

    public void BuildingCleanStateEnter()
    {
        ruinUnit.gameObject.SetActive(false);
        ruinUnit.SetRuinActive(false);
    }

    public void BrokenBuilding(ForwardType forwardType)
    {
        switch (currentStateType)
        {
            case BrokenBuildingStateType.Unbroken:
                ToBroken(forwardType);
                break;
            case BrokenBuildingStateType.Broken:
                ToRuin(forwardType);
                break;
            case BrokenBuildingStateType.Ruin:
                break;
            case BrokenBuildingStateType.Clean:
                break;
        }
    }

    private void ToBroken(ForwardType forwardType)
    {
        currentStateType = BrokenBuildingStateType.Broken;
        _cacheForward = forwardType;
        _fsm.ChangeFsmState(typeof(BuildingBrokenState));
    }
    
    private void ToRuin(ForwardType forwardType)
    {
        walkable = true;
        currentStateType = BrokenBuildingStateType.Ruin;
        Vector3 targetRuinPosition = transform.position;
        float cellSize = GridManager.Instance.GetPerCellSize();
        switch (forwardType)
        {
            case ForwardType.Up:
                targetRuinPosition += new Vector3(0, 0, cellSize);
                break;
            case ForwardType.Down:
                targetRuinPosition += new Vector3(0, 0, -cellSize);
                break;
            case ForwardType.Left:
                targetRuinPosition += new Vector3(-cellSize, 0, 0);
                break;
            case ForwardType.Right:
                targetRuinPosition += new Vector3(cellSize, 0, 0);
                break;
        }

        ruinUnit.gameObject.transform.position = targetRuinPosition;
        BattleManager.Instance.UpdateUnitPoint(ruinUnit,GridManager.Instance.GetPointByWorldPosition(targetRuinPosition));
        _fsm.ChangeFsmState(typeof(BuildingRuinState));
    }

    private void ToClean()
    {
        currentStateType = BrokenBuildingStateType.Clean;
        _fsm.ChangeFsmState(typeof(BuildingCleanState));
    }

    private void InitFsm()
    {
        _fsm = new FsmComponent<BattleUnitBrokenBuilding>(this);
        Dictionary<Type, IFsmComponentState<BattleUnitBrokenBuilding>> fsmStates = new Dictionary<Type, IFsmComponentState<BattleUnitBrokenBuilding>>();
        fsmStates.Add(typeof(BuildingUnbrokenState), new BuildingUnbrokenState());
        fsmStates.Add(typeof(BuildingBrokenState), new BuildingBrokenState());
        fsmStates.Add(typeof(BuildingRuinState), new BuildingRuinState());
        fsmStates.Add(typeof(BuildingCleanState), new BuildingCleanState());
        _fsm.SetFsm(fsmStates);
        //fsm此时处于挂起状态，没有state
    }

    private void ResetAll()
    {
        walkable = false;
        currentStateType = originalStateType;
        _cacheForward = forwardForBrokenAtStart;
        _cacheTimeToRuin = timeToRuin;
        _cacheTimeToClean = timeToClean;
        ruinUnit.SetRuinActive(false);
        ResetFsm();
    }
    
    private void ResetFsm()
    {
        switch (currentStateType)
        {
            case BrokenBuildingStateType.Unbroken:
            case BrokenBuildingStateType.Clean:
            case BrokenBuildingStateType.Ruin:
                _fsm.ChangeFsmState(typeof(BuildingUnbrokenState));
                break;
            case BrokenBuildingStateType.Broken:
                _fsm.ChangeFsmState(typeof(BuildingBrokenState));
                break;
        }
    }
}