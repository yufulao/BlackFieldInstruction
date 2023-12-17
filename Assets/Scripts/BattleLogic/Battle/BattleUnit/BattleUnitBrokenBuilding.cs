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
    
    /// <summary>
    /// 获取是否这一格是否可以移动
    /// </summary>
    /// <returns></returns>
    public bool GetWalkable()
    {
        return currentStateType == BrokenBuildingStateType.Ruin || currentStateType == BrokenBuildingStateType.Clean;
    }

    /// <summary>
    /// 完好建筑状态enter
    /// </summary>
    public void BuildingUnbrokenStateEnter()
    {
        unBrokenBuilding.SetActive(true);
        brokenBuilding.SetActive(false);
    }

    /// <summary>
    /// 破损建筑状态enter
    /// </summary>
    public void BuildingBrokenStateEnter()
    {
        unBrokenBuilding.SetActive(false);
        brokenBuilding.SetActive(true);
    }

    /// <summary>
    /// 废墟状态enter
    /// </summary>
    public void BuildingRuinStateEnter()
    {
        brokenBuilding.SetActive(false);
        ruinUnit.SetRuinActive(true);
    }

    /// <summary>
    /// 废墟清除后状态enter
    /// </summary>
    public void BuildingCleanStateEnter()
    {
        ruinUnit.gameObject.SetActive(false);
        ruinUnit.SetRuinActive(false);
    }

    /// <summary>
    /// 破坏建筑
    /// </summary>
    /// <param name="forwardType"></param>
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

    /// <summary>
    /// 完好建筑变到破损建筑
    /// </summary>
    /// <param name="forwardType"></param>
    private void ToBroken(ForwardType forwardType)
    {
        currentStateType = BrokenBuildingStateType.Broken;
        _cacheForward = forwardType;
        _fsm.ChangeFsmState(typeof(BuildingBrokenState));
    }
    
    /// <summary>
    /// 破损建筑变到废墟
    /// </summary>
    /// <param name="forwardType"></param>
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

    /// <summary>
    /// 废墟变到清除
    /// </summary>
    private void ToClean()
    {
        currentStateType = BrokenBuildingStateType.Clean;
        _fsm.ChangeFsmState(typeof(BuildingCleanState));
    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
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

    /// <summary>
    /// 重置unit
    /// </summary>
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
    
    /// <summary>
    /// 重置状态机
    /// </summary>
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