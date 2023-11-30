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
    
    private List<IEnumerator> _usedCommandList;
    private List<IEnumerator> _excuteCommandList;

    public void InitCommandManager(RowCfgStage rowCfgStage)
    {
        _rowCfgStage = rowCfgStage;
        _usedCommandList = new List<IEnumerator>();
        _excuteCommandList = new List<IEnumerator>();
        _viewCtrl =UIManager.Instance.GetCtrl("CommandView",rowCfgStage) as CommandViewCtrl;
    }
    
    public void OpenCommandView()
    {
        UIManager.Instance.OpenWindow("CommandView");
        
    }
}
