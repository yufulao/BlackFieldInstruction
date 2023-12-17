using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.UI;

public class CommandManager : MonoSingleton<CommandManager>
{
    private CommandUICtrl _viewCtrl;
    private RowCfgStage _rowCfgStage;
    private BattleUnitPlayer _player;
    private readonly List<CommandType> _usedCommandList = new List<CommandType>();
    private CommandFsm _commandFsm;
    private Coroutine _fsmCoroutine;

    private readonly List<Coroutine> _calculateList = new List<Coroutine>();
    private readonly List<Coroutine> _executeList = new List<Coroutine>();
    private readonly List<Coroutine> _checkOverlapList = new List<Coroutine>();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="rowCfgStage"></param>
    public void InitCommandManager(RowCfgStage rowCfgStage, BattleUnitPlayer player)
    {
        _rowCfgStage = rowCfgStage;
        _player = player;
        _usedCommandList.Clear();
        _viewCtrl = UIManager.Instance.GetCtrl<CommandUICtrl>("CommandView", rowCfgStage);
        InitCommandFsm(); //挂起状态
    }

    /// <summary>
    /// 打开Command页面
    /// </summary>
    public void CommandUIOnBeginExecuteCommand()
    {
        _viewCtrl.CommandUIOnBeginBattleExecuteCommand();
    }

    /// <summary>
    /// 关闭command页面
    /// </summary>
    public void CommandUIOnEndBattle()
    {
        _viewCtrl.CommandUIOnEndBattle();
    }

    /// <summary>
    /// 开始执行指令集
    /// </summary>
    public void OnExecuteCommandStart()
    {
        _commandFsm.ChangeFsmState(typeof(CommandMainStartState));
    }

    /// <summary>
    /// 第一条指令开始执行前的状态enter
    /// </summary>
    public void CommandMainStartStateEnter()
    {
        PrepareCommand();
        EventManager.Instance.Dispatch(EventName.CommandMainStart);
        _commandFsm.ChangeFsmState(typeof(CommandCalculateState));
    }

    /// <summary>
    /// unit计算targetPoint阶段的enter
    /// </summary>
    public void CommandCalculateStateEnter()
    {
        _fsmCoroutine = StartCoroutine(ICommandCalculateStateEnter());
    }

    /// <summary>
    /// 等待全部unit计算结束时，进入指令执行开始时状态
    /// </summary>
    /// <returns></returns>
    private IEnumerator ICommandCalculateStateEnter()
    {
        _calculateList.Clear();
        List<BattleUnit> allUnit = BattleManager.Instance.GetAllUnit();
        for (int i = 0; i < allUnit.Count; i++)
        {
            _calculateList.Add(StartCoroutine(allUnit[i].Calculate(_usedCommandList[0])));
            //Debug.Log("计算指令"+_usedCommandList[0]);
        }
        _usedCommandList.RemoveAt(0);
        
        // 等待所有单位协程执行完毕
        foreach (var coroutine in _calculateList)
        {
            yield return coroutine;
        }

        //Debug.Log("所有unit都计算完毕");
        _commandFsm.ChangeFsmState(typeof(CommandExecuteStartState));
    }

    /// <summary>
    /// 指令开始执行时的状态，暂无，直接跳转到指令执行中阶段
    /// </summary>
    public void CommandExecuteStartStateEnter()
    {
        _commandFsm.ChangeFsmState(typeof(CommandExecutingState));
    }

    /// <summary>
    /// 指令执行中阶段的enter，相当于开始执行指令时
    /// </summary>
    public void CommandExecutingStateEnter()
    {
        _fsmCoroutine = StartCoroutine(ICommandExecutingStateEnter());
    }

    /// <summary>
    /// 等待所有unit都执行完指令，才进入检查重叠unit，检测特定unit阶段
    /// </summary>
    /// <returns></returns>
    private IEnumerator ICommandExecutingStateEnter()
    {
        _executeList.Clear();
        List<BattleUnit> allUnit = BattleManager.Instance.GetAllUnit();
        for (int i = 0; i < allUnit.Count; i++)
        {
            _executeList.Add(StartCoroutine(allUnit[i].Execute()));
        }

        // 等待所有单位协程执行完毕
        foreach (var coroutine in _executeList)
        {
            yield return coroutine;
        }

        //Debug.Log("所有unit都执行完毕");
        _commandFsm.ChangeFsmState(typeof(CommandCheckOverlapState));
    }

    /// <summary>
    /// unit检测特定unit阶段enter
    /// </summary>
    public void CommandCheckOverlapStateEnter()
    {
        _fsmCoroutine = StartCoroutine(ICommandCheckOverlapStateEnter());
    }

    /// <summary>
    /// 等待所有unit都检测完特定unit才进入指令执行结束阶段
    /// </summary>
    /// <returns></returns>
    private IEnumerator ICommandCheckOverlapStateEnter()
    {
        _checkOverlapList.Clear();
        List<BattleUnit> allUnit = BattleManager.Instance.GetAllUnit();
        for (int i = 0; i < allUnit.Count; i++)
        {
            _checkOverlapList.Add(StartCoroutine(allUnit[i].CheckOverlap()));
        }

        // 等待所有单位协程执行完毕
        foreach (var coroutine in _checkOverlapList)
        {
            yield return coroutine;
        }

        //Debug.Log("所有unit都检测重叠完毕");
        _commandFsm.ChangeFsmState(typeof(CommandExecuteEndState));
    }

    /// <summary>
    /// 指令执行结束阶段的enter
    /// </summary>
    public void CommandExecuteEndStateEnter()
    {
        if (_viewCtrl.RefreshCacheCurrentTimeTextInExecuting()) //更新左上角时间，如果超时
        {
            _commandFsm.ChangeFsmState(typeof(CommandMainEndState));
            BattleManager.Instance.BattleEnd(false);
            return;
        }

        if (_usedCommandList.Count <= 0) //指令全部执行完毕
        {
            _commandFsm.ChangeFsmState(typeof(CommandMainEndState));
            BattleManager.Instance.BattleEnd(BattleManager.Instance.CheckPlayerGetTarget());
            return;
        }

        //执行下一个指令
        _commandFsm.ChangeFsmState(typeof(CommandCalculateState));
    }

    /// <summary>
    /// 所有指令都执行完毕时
    /// </summary>
    public void CommandMainEndStateEnter()
    {
        if (_fsmCoroutine != null)
        {
            StopCoroutine(_fsmCoroutine);
        }
    }

    /// <summary>
    /// 强制停止指令执行阶段，切换到所有指令都执行结束阶段
    /// </summary>
    public void ForceChangeToMainEnd()
    {
        _commandFsm.ChangeFsmState(typeof(CommandMainEndState));
    }

    /// <summary>
    /// 预处理指令集，把usedCommand里所有的指令都加载到list中
    /// </summary>
    private void PrepareCommand()
    {
        _usedCommandList.Clear();
        List<UsedItemInfo> usedItemInfoList = _viewCtrl.GetAllUsedItem();
        for (int i = 0; i < usedItemInfoList.Count; i++)
        {
            for (int j = 0; j < usedItemInfoList[i].cacheCount; j++)
            {
                for (int k = 0; k < usedItemInfoList[i].cacheTime; k++)
                {
                    _usedCommandList.Add(usedItemInfoList[i].cacheCommandEnum);
                    //Debug.Log(usedObjList[i].commandList[j]);
                }
            }
        }

        //Debug.Log(_usedCommandList.Count);
    }

    /// <summary>
    /// 初始化commandManager的状态机
    /// </summary>
    private void InitCommandFsm()
    {
        _commandFsm = FsmManager.Instance.GetFsmByName<CommandFsm>("CommandManager");
        Dictionary<Type, IFsmState> commandFsmStates = new Dictionary<Type, IFsmState>();
        commandFsmStates.Add(typeof(CommandMainStartState), new CommandMainStartState());
        commandFsmStates.Add(typeof(CommandCalculateState), new CommandCalculateState());
        commandFsmStates.Add(typeof(CommandExecuteStartState), new CommandExecuteStartState());
        commandFsmStates.Add(typeof(CommandExecutingState), new CommandExecutingState());
        commandFsmStates.Add(typeof(CommandCheckOverlapState), new CommandCheckOverlapState());
        commandFsmStates.Add(typeof(CommandExecuteEndState), new CommandExecuteEndState());
        commandFsmStates.Add(typeof(CommandMainEndState), new CommandMainEndState());
        _commandFsm.SetFsm(commandFsmStates);
        //fsm此时处于挂起状态，没有state
    }
}