using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : BaseSingleTon<BattleManager>,IMonoManager
{
    private RowCfgStage _rowCfgStage;
    private BattleFsm _battleFsm;
    private bool _win;
    private BattleModel _model;
    private BattleView _view;
    private BattleUnitPlayer _player;
    private readonly List<BattleUnitTarget> _targetUnits = new List<BattleUnitTarget>();
    private readonly List<BattleUnit> _allUnits = new List<BattleUnit>();
    private Dictionary<BattleUnit, BattleUnitInfo> _unitInfoDic = new Dictionary<BattleUnit, BattleUnitInfo>();
    private bool _hasInit;
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

    public void ForceStopExcuteCommand()
    {
        CommandManager.Instance.StopAllCoroutines();
        _player.ForceStopPlayerTweener();
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
    public void ChangeToCommandExcuteState()
    {
        _battleFsm.ChangeFsmState(typeof(BattleCommandExcuteState));
    }

    /// <summary>
    /// battleCommandExcute阶段的enter
    /// </summary>
    public void BattleCommandExcuteStateEnter()
    {
        CommandManager.Instance.CommandUIOnBeginExcuteCommand();
        CommandManager.Instance.OnExcuteCommandStart();
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

    /// <summary>
    /// player每次Move之后检测是否抵达目标，以及当指令结束完毕时再调用一次计算本轮最终结果
    /// </summary>
    public bool CheckPlayerGetTarget()
    {
        for (int i = 0; i < _targetUnits.Count; i++)
        {
            if (_model.IsSameCurrentPoint(_unitInfoDic[_player], _unitInfoDic[_targetUnits[i]]))
            {
                BattleEnd(true);
                return true;
            }
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
            BattleUnitInfo unitInfo = _model.CreatUnitInfo(objPoint);
            _allUnits.Add(gridObjList[i]);
            _model.AddGridCellUnitInfo(objPoint.x, objPoint.y, unitInfo);

            //初始化数据
            gridObjList[i].OnInit();

            if (gridObjList[i] is BattleUnitPlayer)
            {
                _player = (BattleUnitPlayer) gridObjList[i];
                _player.InitPlayer(unitInfo);//player只能读取info不能修改info
            }

            if (gridObjList[i] is BattleUnitTarget)
            {
                _targetUnits.Add((BattleUnitTarget) gridObjList[i]);
            }

            if (_unitInfoDic.ContainsKey(gridObjList[i]))
            {
                _unitInfoDic[gridObjList[i]] = unitInfo;
                continue;
            }

            _unitInfoDic.Add(gridObjList[i], unitInfo);
        }

        if (_player == null || _targetUnits.Count == 0)
        {
            Debug.LogError("场景中没有player或目标");
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
            _allUnits[i].OnReset();
            BattleUnitInfo unitInfo = _unitInfoDic[_allUnits[i]];
            if (_model.IsNoMove(unitInfo))
            {
                continue;
            }

            _allUnits[i].gameObject.transform.position = GridManager.Instance.GetWorldPositionByPoint(unitInfo.originalPoint.x, unitInfo.originalPoint.y);
            //Debug.Log(_allUnits[i].gameObject.name);
            _model.ResetUnitInfo(unitInfo);
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
}