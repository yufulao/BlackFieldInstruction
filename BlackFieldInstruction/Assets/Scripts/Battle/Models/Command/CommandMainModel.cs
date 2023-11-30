using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class CommandMainModel : MonoBehaviour
{
    private List<IEnumerator> _usedCommandList;
    private List<IEnumerator> _excuteCommandList;
    private List<UsedCommandObj> _usedObjList;
    private List<WaitingCommandObj> _waitingObjList;

    private int _currentNeedTime;


    /// <summary>
    /// 初始化本关卡所有可选指令
    /// </summary>
    /// <param name="originalObjCommandList">初始指令列表</param>
    /// <returns></returns>
    public void InitModel(List<WaitingCommandObj> originalObjCommandList)
    {
        _usedCommandList = new List<IEnumerator>();
        _excuteCommandList = new List<IEnumerator>();
        _usedObjList = new List<UsedCommandObj>();
        _waitingObjList = new List<WaitingCommandObj>();
        _currentNeedTime = 0;
        _waitingObjList = originalObjCommandList;
    }

    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingObj"></param>
    public void ClickWaitingObj(WaitingCommandObj waitingObj)
    {
        _currentNeedTime+=waitingObj.needTime;
        RemoveWaitingObj(waitingObj);
        AddUsedObj(waitingObj);
    }

    public void ClickUsedObj(UsedCommandObj usedObj)
    {
        WaitingCommandObj waitingObj= AddWaitingObj(usedObj.commandEnum);
        _currentNeedTime-=waitingObj.needTime;
        RemoveUsedObj(usedObj);
    }

    private void AddUsedObj(WaitingCommandObj waitingObj)
    {
        if (_usedObjList.Count==0||_usedObjList[_usedObjList.Count - 1].commandEnum!=waitingObj.commandEnum)
        {
            StartCoroutine(CommandManager.Instance.AddNewUsedObj(waitingObj.commandEnum, waitingObj.needTime,
                (newUsedObj) =>
                {
                    _usedObjList.Add(newUsedObj);
                    UpdateLastUsedObj();
                }));
            return;
        }
        
        //最新的usedObj是同类command
        {
            _usedObjList[_usedObjList.Count - 1].count++;
            UpdateLastUsedObj();
        }
    }
    
    /// <summary>
    /// waitingObj加1
    /// </summary>
    /// <param name="commandEnum"></param>
    private WaitingCommandObj AddWaitingObj(CommandEnum commandEnum)
    {
        WaitingCommandObj waitingObj=null;
        for (int i = 0; i < _waitingObjList.Count; i++)
        {
            if (_waitingObjList[i].commandEnum == commandEnum)
            {
                waitingObj = _waitingObjList[i];
                break;
            }
        }
        
        if (waitingObj==null)
        {
            Debug.LogError("waitingList里没有这个commandEnum"+commandEnum);
            return null;
        }
        waitingObj.count++;
        CommandManager.Instance.UpdateWaitingObjView(waitingObj);
        return waitingObj;
    }
    
    /// <summary>
    /// waitingObj减一
    /// </summary>
    /// <param name="waitingObj"></param>
    private void RemoveWaitingObj(WaitingCommandObj waitingObj)
    {
        waitingObj.count--;
        CommandManager.Instance.UpdateWaitingObjView(waitingObj);
        if (_waitingObjList.Count<=0)
        {
            CommandManager.Instance.NoWaitingObj();
        }
    }

    private void RemoveUsedObj(UsedCommandObj usedObj)
    {
        usedObj.count--;
        if (usedObj.count<=0)
        {
            _usedObjList.Remove(usedObj);
            Destroy(usedObj.gameObject);//对象池处理===================================================================================
        }
        
        if (_usedObjList.Count<=0)
        {
            CommandManager.Instance.NoUsedObj();
        }
        UpdateLastUsedObj();
    }

    /// <summary>
    /// 更新最新的usedObj的当前时间
    /// </summary>
    private void UpdateLastUsedObj()
    {
        if (_usedObjList.Count==0)
        {
            return;
        }

        UsedCommandObj lastUsedObj = _usedObjList[_usedObjList.Count - 1];
        lastUsedObj.currentTime = _currentNeedTime;
        CommandManager.Instance.UpdateLastUsedObjView(lastUsedObj);
        return;
    }
}
