using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitBrokenBuilding : BattleUnit
{
    [SerializeField] private BrokenBuildingStateType originalStateType;
    [SerializeField] private float timeFromRuinToClean;
    [HideInInspector] public BrokenBuildingStateType currentStateType;

    [SerializeField] private BattleUnitRuin ruinUnit;
    [SerializeField] private GameObject unBrokenBuilding;
    [SerializeField] private GameObject brokenBuilding;


    private FsmComponent<BattleUnitBrokenBuilding> _fsm;

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
        StartCoroutine(RuinToClean());
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
                UnbrokenToBroken();
                break;
            case BrokenBuildingStateType.Broken:
                BrokenToRuin(forwardType);
                break;
            case BrokenBuildingStateType.Ruin:
                break;
            case BrokenBuildingStateType.Clean:
                break;
        }
    }

    public bool GetWalkable()
    {
        return currentStateType == BrokenBuildingStateType.Ruin || currentStateType == BrokenBuildingStateType.Clean;
    }

    private void UnbrokenToBroken()
    {
        currentStateType = BrokenBuildingStateType.Broken;
        _fsm.ChangeFsmState(typeof(BuildingBrokenState));
    }

    private void BrokenToRuin(ForwardType forwardType)
    {
        brokenBuilding.SetActive(false);
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
        ruinUnit.SetRuinActive(true);
        BattleManager.Instance.UpdateUnitPoint(ruinUnit);
        ruinUnit.CheckCellAfterActive();
        _fsm.ChangeFsmState(typeof(BuildingRuinState));
    }

    private IEnumerator RuinToClean()
    {
        yield return new WaitForSeconds(timeFromRuinToClean);
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
        StopAllCoroutines();
        walkable = false;
        currentStateType = originalStateType;
        ruinUnit.SetRuinActive(false);
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