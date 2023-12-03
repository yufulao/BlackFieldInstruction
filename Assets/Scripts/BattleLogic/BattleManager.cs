using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager:MonoSingleton<BattleManager>
{
    private RowCfgStage _currentStage;
    private BattleFsm _battleFsm;
    private bool _win;

    private void Update()
    {
        _battleFsm?.OnUpdate();
    }
    
    /// <summary>
    /// 初始化BattleManager
    /// </summary>
    public void OnInit()
    {
        InitBattleParams();
        InitBattleFsm();
    }

    /// <summary>
    /// 进入battle场景
    /// </summary>
    /// <param name="stageName"></param>
    public void EnterStageScene(string stageName)
    {
        LoadStageCfg(stageName);
        _battleFsm.ChangeFsmState(typeof(BattleInitState));
    }

    /// <summary>
    /// BattleInit阶段的enter
    /// </summary>
    public void BattleInitStateEnter()
    {
        CommandManager.Instance.InitCommandManager(_currentStage);
        //理论上，协程结束后场景已经切换，所以旧的被销毁，新的被识别为instance……吧？嗯？
        GridManager.Instance.LoadGridManager(_currentStage); //重新获取场景中的model和view
        
        _battleFsm.ChangeFsmState(typeof(BattleCommandInputState));
    }

    /// <summary>
    /// battleInput阶段的enter
    /// </summary>
    public void BattleInputStateEnter()
    {
        CommandManager.Instance.OpenCommandView();
    }

    /// <summary>
    /// 开始执行指令，预留给commandUICtrl注册_startBtn的click监听
    /// </summary>
    public void ChangeToCommandExcuteState()
    {
        CommandManager.Instance.CloseCommandView();
        _battleFsm.ChangeFsmState(typeof(BattleCommandExcuteState));
    }

    /// <summary>
    /// battleCommandExcute阶段的enter
    /// </summary>
    public void BattleCommandExcuteStateEnter()
    {
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
        Debug.Log("游戏结束"+_win);
        if (!_win)
        {
            ResetBattle();
        }
    }

    /// <summary>
    /// 重置战斗btn事件
    /// </summary>
    private void ResetBattle()
    {
        ResetBattleParams();
        GridManager.Instance.ResetGridModel();
        _battleFsm.ChangeFsmState(typeof(BattleCommandInputState));
    }
    
    /// <summary>
    /// 加载场景配置
    /// </summary>
    /// <param name="stageName"></param>
    private void LoadStageCfg(string stageName)
    {
        _currentStage=ConfigManager.Instance.cfgStage[stageName];
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
        battleFsmStates.Add(typeof(BattleInitState),new BattleInitState());
        battleFsmStates.Add(typeof(BattleCommandInputState),new BattleCommandInputState());
        battleFsmStates.Add(typeof(BattleCommandExcuteState),new BattleCommandExcuteState());
        battleFsmStates.Add(typeof(BattleEndState),new BattleEndState());
        battleFsmStates.Add(typeof(BattlePauseState),new BattlePauseState());
        _battleFsm.SetFsm(battleFsmStates);
        //fsm此时处于挂起状态，没有state
    }
    
}
