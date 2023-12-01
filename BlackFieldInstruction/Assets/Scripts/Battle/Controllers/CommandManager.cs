using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.UI;

public class CommandManager : MonoSingleton<CommandManager>
{
    private CommandViewCtrl _viewCtrl;
    private RowCfgStage _rowCfgStage;

    private readonly List<IEnumerator> _usedCommandList=new List<IEnumerator>();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="rowCfgStage"></param>
    public void InitCommandManager(RowCfgStage rowCfgStage)
    {
        _rowCfgStage = rowCfgStage;
        _usedCommandList.Clear();
        _viewCtrl = UIManager.Instance.GetCtrl("CommandView", rowCfgStage) as CommandViewCtrl;
    }

    /// <summary>
    /// 打开Command页面
    /// </summary>
    public void OpenCommandView()
    {
        UIManager.Instance.OpenWindow("CommandView");
    }

    /// <summary>
    /// 关闭command页面
    /// </summary>
    public void CloseCommandView()
    {
        UIManager.Instance.CloseWindows("CommandView");
    }

    /// <summary>
    /// 添加指令到usedObj的commandList
    /// </summary>
    /// <param name="commandType"></param>
    public void AddCommand(CommandType commandType)
    {
        Player player = GridManager.Instance.player;

        switch (commandType)
        {
            case CommandType.Up:
                _usedCommandList.Add(player.MoveCommand(CommandType.Up)); 
                break;
            case CommandType.Down:
                _usedCommandList.Add( player.MoveCommand(CommandType.Down));
                break;
            case CommandType.Right:
                _usedCommandList.Add( player.MoveCommand(CommandType.Right));
                break;
            case CommandType.Left:
                _usedCommandList.Add( player.MoveCommand(CommandType.Left));
                break;
            case CommandType.Wait:
                _usedCommandList.Add( player.MoveCommand(CommandType.Wait));
                _usedCommandList.Add(player.MoveCommand(CommandType.Wait));
                break;
            default:
                Debug.Log("没有添加这个command的方法" + commandType.ToString());
                break;
        }
    }

    /// <summary>
    /// 开始执行指令集
    /// </summary>
    public void OnExcuteCommandStart()
    {
        PrepareCommand();
        ExcuteCommand();
    }

    /// <summary>
    /// 执行当前指令
    /// </summary>
    /// <param name="end"></param>
    public void ExcuteCommand()
    {
        if (_usedCommandList.Count != 0)
        {
            IEnumerator command =  _usedCommandList[0];
            //Debug.Log("执行指令"+_usedCommandList[0]);
            StartCoroutine(command);
            _usedCommandList.RemoveAt(0);
            return;
        }

        OnExcuteCommandEnd();
    }

    /// <summary>
    /// 预处理指令集
    /// </summary>
    private void PrepareCommand()
    {
        _usedCommandList.Clear();
        List<UsedCommandObj> usedObjList = _viewCtrl.GetAllUsedObj();
        for (int i = 0; i < usedObjList.Count; i++)
        {
            for (int j = 0; j < usedObjList[i].count; j++)
            {
                //Debug.Log(usedObjList[i].commandList[j]);
                AddCommand(usedObjList[i].commandEnum);
            }
        }
        //Debug.Log(_usedCommandList.Count);
    }

    /// <summary>
    /// 所有指令执行完毕
    /// </summary>
    private void OnExcuteCommandEnd()
    {
        BattleManager.Instance.BattleEnd(GridManager.Instance.CheckPlayerGetTarget());
    }
}