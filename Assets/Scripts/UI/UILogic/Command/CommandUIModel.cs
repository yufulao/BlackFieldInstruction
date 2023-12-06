using System;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class CommandUIModel : MonoBehaviour
{
    private RowCfgStage _rowCfgStage;
    private List<CommandItemInfo> _usedItemInfoList;
    private List<CommandItemInfo> _waitingItemInfoList;

    private int _currentNeedTime;


    /// <summary>
    /// 初始化本关卡所有可选指令
    /// </summary>
    /// <param name="originalItemList">初始指令列表</param>
    /// <param name="rowCfgStage">关卡数据</param>
    /// <returns></returns>
    public void OnInit(List<CommandItemInfo> originalItemList, RowCfgStage rowCfgStage)
    {
        _rowCfgStage = rowCfgStage;
        _usedItemInfoList = new List<CommandItemInfo>();
        _waitingItemInfoList = new List<CommandItemInfo>();
        _currentNeedTime = 0;
        _waitingItemInfoList = originalItemList;
    }

    /// <summary>
    /// 获取usedCommandList
    /// </summary>
    /// <returns></returns>
    public List<CommandItemInfo> GetUsedItemList()
    {
        List<CommandItemInfo> usedItemInfoList = new List<CommandItemInfo>();
        for (int i = 0; i < _usedItemInfoList.Count; i++)
        {
            usedItemInfoList.Add(_usedItemInfoList[i]);
        }

        return usedItemInfoList;
    }

    /// <summary>
    /// 获取当前总时间
    /// </summary>
    /// <returns></returns>
    public int GetCurrentNeedTime()
    {
        return _currentNeedTime;
    }

    /// <summary>
    /// 尝试添加usedList末尾相同的ItemInfo
    /// </summary>
    /// <param name="waitingItemInfo"></param>
    /// <returns></returns>
    public bool TryAddSameUsedCommand(CommandItemInfo waitingItemInfo)
    {
        if (_usedItemInfoList.Count != 0 && _usedItemInfoList[^1].cacheCommandEnum == waitingItemInfo.cacheCommandEnum)
        {
            //最新的usedObj是同类command
            _currentNeedTime += waitingItemInfo.cacheTime;
            CommandItemInfo usedItemInfo = _usedItemInfoList[^1];
            usedItemInfo.cacheCount++;
            SetUsedItem(usedItemInfo, waitingItemInfo.cacheTime);
            return true;
        }

        return false;
    }

    /// <summary>
    /// usedItem加一
    /// </summary>
    /// <param name="waitingItemInfo"></param>
    /// <param name="usedItemInfo"></param>
    public void AddUsedCommand(CommandItemInfo waitingItemInfo, CommandItemInfo usedItemInfo)
    {
        _currentNeedTime += waitingItemInfo.cacheTime;
        _usedItemInfoList.Add(usedItemInfo);
        SetUsedItem(usedItemInfo, waitingItemInfo.cacheTime);
    }

    /// <summary>
    /// usedItem减一
    /// </summary>
    /// <param name="usedItemInfo"></param>
    /// <param name="noUsedObjCallback"></param>
    public void RemoveUsedCommand(CommandItemInfo usedItemInfo, Action noUsedObjCallback)
    {
        int timeAddon = -usedItemInfo.cacheTime;
        _currentNeedTime += timeAddon;
        usedItemInfo.cacheCount--;
        
        int removeUsedItemIndex = _usedItemInfoList.IndexOf(usedItemInfo);
        SetUsedItem(usedItemInfo, timeAddon);
        if (usedItemInfo.cacheCount<=0)
        {
            CombineSameUsedItem(removeUsedItemIndex);
        }

        if (_usedItemInfoList.Count <= 0)
        {
            noUsedObjCallback.Invoke();
        }
    }

    /// <summary>
    /// waitingItem加1
    /// </summary>
    /// <param name="usedItemInfo"></param>
    public CommandItemInfo AddWaitingCommand(CommandItemInfo usedItemInfo)
    {
        CommandItemInfo waitingItemInfo = null;
        for (int i = 0; i < _waitingItemInfoList.Count; i++)
        {
            if (_waitingItemInfoList[i].cacheCommandEnum == usedItemInfo.cacheCommandEnum)
            {
                waitingItemInfo = _waitingItemInfoList[i];
                break;
            }
        }

        if (waitingItemInfo == null)
        {
            Debug.Log("找不到对应的waitingItem");
            return null;
        }

        waitingItemInfo.cacheCount++;
        return waitingItemInfo;
    }

    /// <summary>
    /// waitingItem减一
    /// </summary>
    /// <param name="waitingItemInfo"></param>
    public void RemoveWaitingCommand(CommandItemInfo waitingItemInfo, Action noWaitingObjCallback)
    {
        waitingItemInfo.cacheCount--;

        if (_waitingItemInfoList.Count <= 0)
        {
            noWaitingObjCallback.Invoke();
        }
    }

    /// <summary>
    /// 移除usedItemInfoList中的元素
    /// </summary>
    /// <param name="usedItemInfo"></param>
    public void RemoveUsedItemInfo(CommandItemInfo usedItemInfo)
    {
        _usedItemInfoList.Remove(usedItemInfo);
    }

    /// <summary>
    /// 检测Index前后的usedItem是否一样，一样则合并
    /// </summary>
    /// <param name="removeUsedItemIndex"></param>
    private void CombineSameUsedItem(int removeUsedItemIndex)
    {
        if (_usedItemInfoList.Count < 2 || removeUsedItemIndex <= 0 || removeUsedItemIndex >= _usedItemInfoList.Count)
        {
            return;
        }

        CommandItemInfo lastUsedItemInfo = _usedItemInfoList[removeUsedItemIndex - 1];
        CommandItemInfo nextUsedItemInfo=_usedItemInfoList[removeUsedItemIndex+1];

        if (lastUsedItemInfo.cacheCommandEnum == nextUsedItemInfo.cacheCommandEnum)
        {
            lastUsedItemInfo.cacheCount += nextUsedItemInfo.cacheCount;
            nextUsedItemInfo.cacheCount = 0;
            lastUsedItemInfo.currentTime = nextUsedItemInfo.currentTime;
        }

        SetUsedItem(lastUsedItemInfo, 0);
    }

    /// <summary>
    /// 更新usedObj的当前时间
    /// </summary>
    /// <param name="usedItemInfo"></param>
    /// <param name="timeAddon"></param>
    private void SetUsedItem(CommandItemInfo usedItemInfo, int timeAddon)
    {
        if (_usedItemInfoList.Count == 0)
        {
            return;
        }

        for (int i = _usedItemInfoList.IndexOf(usedItemInfo); i < _usedItemInfoList.Count; i++)
        {
            _usedItemInfoList[i].currentTime += timeAddon;
        }
    }
}