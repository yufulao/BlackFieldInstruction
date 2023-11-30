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

    private void Update()
    {
        _battleFsm?.OnUpdate();
    }
    
    /// <summary>
    /// 初始化BattleManager
    /// </summary>
    public void OnInit()
    {
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

    public void BattleInitStateEnter()
    {
        CommandManager.Instance.ReloadCommandModel(_currentStage);//可以只更新model，view复用
        //理论上，协程结束后场景已经切换，所以旧的被销毁，新的被识别为instance……吧？嗯？
        GridManager.Instance.LoadGridManager(_currentStage); //重新获取场景中的model和view
        
        _battleFsm.ChangeFsmState(typeof(BattleCommandInputState));
    }

    public void BattleInputStateEnter()
    {
        CommandManager.Instance.OpenCommandView();
    }

    /// <summary>
    /// 指令输入阶段Update监听
    /// </summary>
    public void BattleInputStateUpdate()
    {
        
    }

    /// <summary>
    /// 结束输入指令btn事件
    /// </summary>
    public void FinishCommandInput()
    {
        _battleFsm.ChangeFsmState(typeof(BattleCommandExcuteState));
    }

    /// <summary>
    /// 结束执行指令阶段
    /// </summary>
    public void FinishCommandExcute()
    {
        _battleFsm.ChangeFsmState(typeof(BattleEndState));
    }

    /// <summary>
    /// 检测战斗结果
    /// </summary>
    /// <returns></returns>
    public bool CheckBattleEnd()
    {
        return false;
    }

    /// <summary>
    /// 重置战斗btn事件，(重新进入场景)
    /// </summary>
    public void ResetBattle()
    {
        StartCoroutine(SceneManager.Instance.ChangeSceneAsync(_currentStage.scenePath, (sceneInstance) =>
        {
            _battleFsm.ChangeFsmState(typeof(BattleCommandInputState));
        }));
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
