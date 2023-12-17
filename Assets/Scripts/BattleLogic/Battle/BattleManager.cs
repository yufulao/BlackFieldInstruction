using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class BattleManager : BaseSingleTon<BattleManager>, IMonoManager
{
    private RowCfgStage _rowCfgStage;
    private BattleFsm _battleFsm;
    private BattleModel _model;
    private BattleView _view;
    private BattleUnitPlayer _player;
    private readonly List<BattleUnitTarget> _targetUnits = new List<BattleUnitTarget>();
    private readonly List<BattleUnit> _allUnits = new List<BattleUnit>();
    private readonly Dictionary<BattleUnit, BattleUnitInfo> _unitInfoDic = new Dictionary<BattleUnit, BattleUnitInfo>();
    private bool _hasInit;
    private bool _win;
    private string _cacheStageName;

    public void OnInit()
    {
        if (_hasInit)
        {
            return;
        }

        InitBattleFsm(); //挂起状态

        _model = new BattleModel();
        _view = GameObject.FindObjectOfType<BattleView>();
        _hasInit = true;
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void OnClear()
    {
    }

    /// <summary>
    /// 进入battle场景
    /// </summary>
    /// <param name="stageName"></param>
    public void EnterStageScene(string stageName)
    {
        _cacheStageName = stageName;
        LoadStageCfg();
        _battleFsm.ChangeFsmState(typeof(BattleInitState));
    }

    /// <summary>
    /// 强制停止指令执行，给commandUICtrl绑定指令执行阶段的取消btn
    /// </summary>
    public void ForceStopExecuteCommand()
    {
        CommandManager.Instance.StopAllCoroutines();
        BattleEnd(false);
    }

    /// <summary>
    /// BattleInit阶段的enter
    /// </summary>
    public void BattleInitStateEnter()
    {
        InitBattleParams();
        GridManager.Instance.InitGridManager(_rowCfgStage); //重新获取场景中的model和view
        LoadUnits();
        CommandManager.Instance.InitCommandManager(_rowCfgStage, _player);
        _battleFsm.ChangeFsmState(typeof(BattleCommandInputState));
    }

    /// <summary>
    /// battleInput阶段的enter
    /// </summary>
    public void BattleInputStateEnter()
    {
        UIManager.Instance.OpenWindow("CommandView");
        CommandManager.Instance.CommandUIOnEndBattle();
    }

    /// <summary>
    /// 开始执行指令，预留给commandUICtrl注册_startBtn的click监听
    /// </summary>
    public void ChangeToCommandExecuteState()
    {
        _battleFsm.ChangeFsmState(typeof(BattleCommandExcuteState));
    }

    /// <summary>
    /// battleCommandExecute阶段的enter
    /// </summary>
    public void BattleCommandExecuteStateEnter()
    {
        CommandManager.Instance.CommandUIOnBeginExecuteCommand();
        CommandManager.Instance.OnExecuteCommandStart();
    }

    /// <summary>
    /// 战斗中结束，传入战斗结果
    /// </summary>
    /// <returns></returns>
    public void BattleEnd(bool win)
    {
        _win = win;
        _battleFsm.ChangeFsmState(typeof(BattleEndState));
    }

    /// <summary>
    /// battleEndState阶段的enter
    /// </summary>
    public void BattleEndStateEnter()
    {
        Debug.Log("游戏结束" + _win);
        if (!_win)
        {
            ResetBattleMap();
        }
    }

    /// <summary>
    /// 检测是否到达target
    /// </summary>
    public bool CheckPlayerGetTarget()
    {
        return CheckCellForUnit<BattleUnit>(_player, UnitType.Target);
    }
    
    /// <summary>
    /// 检测格子是否可走
    /// </summary>
    /// <returns></returns>
    public bool CheckWalkable(int x, int z)
    {
        if (!GridManager.Instance.CheckPointValid(x, z))
        {
            return false;
        }

        GridCell gridCell = GridManager.Instance.GetGridCell(x, z);
        if (!_model.IsContainsGridCell(gridCell))
        {
            return true;
        }

        List<BattleUnitInfo> unitInfos = _model.GetUnitInfoByGridCell(gridCell);
        if (unitInfos == null)
        {
            return true;
        }

        for (int i = 0; i < unitInfos.Count; i++)
        {
            if (!GetUnitByUnitInfo(unitInfos[i]).walkable)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckWalkable(Vector2Int point)
    {
        return CheckWalkable(point.x, point.y);
    }

    public bool CheckCellForUnit<T>(BattleUnit unit, UnitType unitType, Action<List<T>> callback = null) where T : BattleUnit
    {
        BattleUnitInfo unitInfo = _unitInfoDic[unit];
        List<BattleUnitInfo> cellInfos = _model.GetUnitInfoByGridCell(GridManager.Instance.GetGridCell(unitInfo.currentPoint));
        if (cellInfos==null)
        {
            return false;
        }
        List<T> results = new List<T>();
        for (int i = 0; i < cellInfos.Count; i++)
        {
            if (cellInfos[i].unitType == unitType)
            {
                results.Add((T) GetUnitByUnitInfo(cellInfos[i]));
            }
        }

        if (results.Count != 0)
        {
            callback?.Invoke(results);
            return true;
        }

        return false;
    }
    
    public bool CheckCellForOrderPoint<T>(Vector2Int orderPoint, UnitType unitType, Action<List<T>> callback = null) where T : BattleUnit
    {
        if (!GridManager.Instance.CheckPointValid(orderPoint.x,orderPoint.y))
        {
            return false;
        }
        List<BattleUnitInfo> cellInfos = _model.GetUnitInfoByGridCell(GridManager.Instance.GetGridCell(orderPoint));
        List<T> results = new List<T>();
        if (cellInfos==null)
        {
            return false;
        }
        for (int i = 0; i < cellInfos.Count; i++)
        {
            if (cellInfos[i].unitType == unitType)
            {
                results.Add((T) GetUnitByUnitInfo(cellInfos[i]));
            }
        }

        if (results.Count != 0)
        {
            callback?.Invoke(results);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 更新单个GridCell
    /// </summary>
    /// <param name="unitInfo">需要更新的gridObject</param>
    /// <param name="lastPoint">更新之前的point坐标</param>
    /// <param name="newPoint">更新之后的point坐标</param>
    public void UpdateUnitPoint(BattleUnitInfo unitInfo, Vector2Int lastPoint, Vector2Int newPoint)
    {
        _model.UpdateUnitPoint(unitInfo, lastPoint, newPoint);
    }
    public void UpdateUnitPoint(BattleUnit unit, Vector2Int newPoint)
    {
        BattleUnitInfo unitInfo = _unitInfoDic[unit];
        _model.UpdateUnitPoint(unitInfo, unitInfo.currentPoint, newPoint);
    }
    public void UpdateUnitPoint(BattleUnit unit)
    {
        BattleUnitInfo info = _unitInfoDic[unit];
        _model.UpdateUnitPoint(info, info.currentPoint, GridManager.Instance.GetPointByWorldPosition(unit.gameObject.transform.position));
    }

    public List<BattleUnit> GetAllUnit()
    {
        return _allUnits;
    }

    /// <summary>
    /// 只重置战斗地图
    /// </summary>
    private void ResetBattleMap()
    {
        ResetBattleParams();
        ResetUnits();
        _battleFsm.ChangeFsmState(typeof(BattleCommandInputState));
    }

    /// <summary>
    /// 加载场景中的BattleUnit
    /// </summary>
    private void LoadUnits()
    {
        _allUnits.Clear();
        _targetUnits.Clear();
        _player = null;
        _unitInfoDic.Clear();
        var gridObjList = GridManager.Instance.GetGridObjContainer().GetComponentsInChildren<BattleUnit>();
        for (int i = 0; i < gridObjList.Length; i++)
        {
            //创建并添加数据
            Vector2Int objPoint = GridManager.Instance.GetPointByWorldPosition(gridObjList[i].transform.position);
            BattleUnitInfo unitInfo = _model.CreatUnitInfo(gridObjList[i].unitType, objPoint);
            _allUnits.Add(gridObjList[i]);
            _model.AddGridCellUnitInfo(objPoint.x, objPoint.y, unitInfo);
            SwitchUnitWhenLoadUnits(gridObjList[i], unitInfo);

            //初始化数据
            gridObjList[i].OnUnitInit();

            if (_unitInfoDic.ContainsKey(gridObjList[i]))
            {
                _unitInfoDic[gridObjList[i]] = unitInfo;
                continue;
            }

            _unitInfoDic.Add(gridObjList[i], unitInfo);
            //Debug.Log(gridObjList[i]+"--->"+unitInfo.unitType);
        }

        if (_player == null || _targetUnits.Count == 0)
        {
            Debug.LogError("场景中没有player或目标");
        }
    }

    /// <summary>
    /// 特定unit的加载事件
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="info"></param>
    private void SwitchUnitWhenLoadUnits(BattleUnit unit, BattleUnitInfo info)
    {
        switch (unit.unitType)
        {
            case UnitType.Player:
                _player = unit.transform.GetComponent<BattleUnitPlayer>();
                //Debug.Log(unit.gameObject.name);
                break;
            case UnitType.Target:
                _targetUnits.Add(unit.transform.GetComponent<BattleUnitTarget>());
                break;
        }
    }

    /// <summary>
    /// 重置所有unit
    /// </summary>
    private void ResetUnits()
    {
        //强制归位
        for (int i = 0; i < _allUnits.Count; i++)
        {
            _allUnits[i].StopAllCoroutines();
            BattleUnitInfo unitInfo = _unitInfoDic[_allUnits[i]];
            _allUnits[i].gameObject.transform.position = GridManager.Instance.GetWorldPositionByPoint(unitInfo.originalPoint.x, unitInfo.originalPoint.y);
            //Debug.Log(_allUnits[i].gameObject.name + "--->" + _allUnits[i].gameObject.transform.position);
            _model.ResetUnitInfo(unitInfo);
            _allUnits[i].OnUnitReset();
        }
    }

    /// <summary>
    /// 加载场景配置
    /// </summary>
    private void LoadStageCfg()
    {
        if (string.IsNullOrEmpty(_cacheStageName))
        {
            Debug.LogError("battleManager的关卡名为空");
            return;
        }

        _rowCfgStage = ConfigManager.Instance.cfgStage[_cacheStageName];
    }

    /// <summary>
    /// 设置战斗的参数
    /// </summary>
    private void InitBattleParams()
    {
        _win = false;
    }

    /// <summary>
    /// 重置战斗参数
    /// </summary>
    private void ResetBattleParams()
    {
        _win = false;
    }

    /// <summary>
    /// 初始化战斗状态机
    /// </summary>
    private void InitBattleFsm()
    {
        _battleFsm = FsmManager.Instance.GetFsmByName<BattleFsm>("BattleManager");
        Dictionary<Type, IFsmState> battleFsmStates = new Dictionary<Type, IFsmState>();
        battleFsmStates.Add(typeof(BattleInitState), new BattleInitState());
        battleFsmStates.Add(typeof(BattleCommandInputState), new BattleCommandInputState());
        battleFsmStates.Add(typeof(BattleCommandExcuteState), new BattleCommandExcuteState());
        battleFsmStates.Add(typeof(BattleEndState), new BattleEndState());
        battleFsmStates.Add(typeof(BattlePauseState), new BattlePauseState());
        _battleFsm.SetFsm(battleFsmStates);
        //fsm此时处于挂起状态，没有state
    }

    /// <summary>
    /// 通过unitInfo获取unit
    /// </summary>
    /// <param name="unitInfo"></param>
    /// <returns></returns>
    private BattleUnit GetUnitByUnitInfo(BattleUnitInfo unitInfo)
    {
        foreach (var pair in _unitInfoDic)
        {
            if (pair.Value == unitInfo)
            {
                return pair.Key;
            }
        }

        Debug.LogWarning("没有这个item的info" + unitInfo);
        return null;
    }

    /// <summary>
    /// 生成一个unit
    /// </summary>
    /// <param name="unitPrefabName"></param>
    /// <param name="unitType"></param>
    /// <param name="point"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T CreateUnit<T>(string unitPrefabName, UnitType unitType, Vector2Int point) where T : BattleUnit
    {
        T unit = GameObject.Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab[unitPrefabName].prefabPath)
            , GridManager.Instance.GetWorldPositionByPoint(point.x, point.y), Quaternion.identity).GetComponent<T>();
        unit.gameObject.transform.SetParent(GridManager.Instance.GetGridObjContainer());
        BattleUnitInfo info = _model.CreatUnitInfo(unitType, point);
        _allUnits.Add(unit);
        _model.AddGridCellUnitInfo(point.x, point.y, info);
        SwitchUnitWhenLoadUnits(unit, info);
        //初始化数据
        unit.OnUnitInit();

        if (_unitInfoDic.ContainsKey(unit))
        {
            _unitInfoDic[unit] = info;
        }
        else
        {
            _unitInfoDic.Add(unit, info);
        }

        return unit;
    }

    /// <summary>
    /// 销毁一个unit
    /// </summary>
    /// <param name="unit"></param>
    private void DestroyUnit(BattleUnit unit) //对象池处理================================================================================================
    {
        BattleUnitInfo info = _unitInfoDic[unit];
        Vector2Int infoCurrentPoint = info.currentPoint;
        _model.RemoveGridCellUnitInfo(infoCurrentPoint.x, infoCurrentPoint.y, info);
        _unitInfoDic.Remove(unit);
        _allUnits.Remove(unit);
        unit.OnUnitDestroy();
        GameObject.Destroy(unit.gameObject);
    }
}