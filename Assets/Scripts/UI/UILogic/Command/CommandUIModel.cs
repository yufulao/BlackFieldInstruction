using System;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class CommandUIModel : MonoBehaviour
{
    private RowCfgStage _rowCfgStage;
    private List<CommandItem> _usedItemList;
    private List<CommandItem> _waitingItemList;
    private Dictionary<CommandItem, CommandItemInfo> _usedItemInfoList;
    private Dictionary<CommandItem, CommandItemInfo> _waitingItemInfoList;

    private int _currentNeedTime;


    /// <summary>
    /// 初始化本关卡所有可选指令
    /// </summary>
    /// <param name="originalItemInfoList">初始指令列表</param>
    /// <param name="rowCfgStage">关卡数据</param>
    /// <returns></returns>
    public void OnInit(Dictionary<CommandItem, CommandItemInfo> originalItemInfoList, RowCfgStage rowCfgStage)
    {
        _rowCfgStage = rowCfgStage;
        _usedItemList = new List<CommandItem>();
        _waitingItemList = new List<CommandItem>();
        _usedItemInfoList = new Dictionary<CommandItem, CommandItemInfo>();
        _waitingItemInfoList = new Dictionary<CommandItem, CommandItemInfo>();
        _currentNeedTime = 0;
        _waitingItemInfoList = originalItemInfoList;
    }

    /// <summary>
    /// 获取usedCommandList
    /// </summary>
    /// <returns></returns>
    public List<CommandItemInfo> GetUsedItemList()
    {
        List<CommandItemInfo> usedItemInfoList = new List<CommandItemInfo>();
        for (int i = 0; i < _usedItemList.Count; i++)
        {
            usedItemInfoList.Add(_usedItemInfoList[_usedItemList[i]]);
        }

        return usedItemInfoList;
    }

    /// <summary>
    /// 获取item的itemInfo
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isUsedItemInfo"></param>
    /// <returns></returns>
    public CommandItemInfo GetCommandInfo(CommandItem item, bool isUsedItemInfo)
    {
        if (isUsedItemInfo)
        {
            return _usedItemInfoList[item];
        }

        return _waitingItemInfoList[item];
    }

    /// <summary>
    /// 获取当前总时间
    /// </summary>
    /// <returns></returns>
    public int GetCurrentNeedTime()
    {
        return _currentNeedTime;
    }

    public CommandItemInfo AddSameUsedCommand(CommandItem waitingItem, Action<CommandItem, CommandItemInfo> usedRefreshCallback)
    {
        CommandItemInfo waitingItemInfo = _waitingItemInfoList[waitingItem];
        _currentNeedTime += waitingItemInfo.cacheTime;

        if (_usedItemList.Count != 0 && _usedItemInfoList[_usedItemList[^1]].cacheCommandEnum == waitingItemInfo.cacheCommandEnum)
        {
            //最新的usedObj是同类command
            CommandItem usedItem = _usedItemList[^1];
            _usedItemInfoList[usedItem].cacheCount++;
            SetUsedItem(usedItem, waitingItemInfo.cacheTime, usedRefreshCallback);
            return null;
        }

        return waitingItemInfo;
    }

    /// <summary>
    /// usedItem加一
    /// </summary>
    /// <param name="waitingItem"></param>
    public void AddUsedCommand(CommandItem waitingItem, CommandItem usedItem, CommandItemInfo usedItemInfo, Action<CommandItem, CommandItemInfo> usedRefreshCallback)
    {
        CommandItemInfo waitingItemInfo = _waitingItemInfoList[waitingItem];
        _currentNeedTime += waitingItemInfo.cacheTime;

        _usedItemList.Add(usedItem);
        _usedItemInfoList.Add(usedItem, usedItemInfo);
        SetUsedItem(usedItem, waitingItemInfo.cacheTime, usedRefreshCallback);
    }

    /// <summary>
    /// usedItem减一
    /// </summary>
    /// <param name="usedItem"></param>
    public void RemoveUsedCommand(CommandItem usedItem, Action noUsedObjCallback, Action<CommandItem, CommandItemInfo> usedRefreshCallback)
    {
        CommandItemInfo usedItemInfo = _usedItemInfoList[usedItem];
        int timeAddon = -usedItemInfo.cacheTime;
        _currentNeedTime += timeAddon;
        usedItemInfo.cacheCount--;
        SetUsedItem(usedItem, timeAddon, usedRefreshCallback);

        if (usedItemInfo.cacheCount <= 0)
        {
            int removeUsedItemIndex = _usedItemList.IndexOf(usedItem);
            _usedItemList.RemoveAt(removeUsedItemIndex);
            Destroy(usedItem.gameObject); //对象池处理==================================
            CheckSameAfterRemoveUsedItem(removeUsedItemIndex, usedRefreshCallback);
        }

        if (_usedItemList.Count <= 0)
        {
            noUsedObjCallback.Invoke();
        }
    }

    /// <summary>
    /// waitingItem加1
    /// </summary>
    /// <param name="usedItem"></param>
    public void AddWaitingCommand(CommandItem usedItem, Action<CommandItem, CommandItemInfo> callback)
    {
        CommandItem waitingItem = null;
        CommandItemInfo waitingItemInfo = null;
        foreach (var waitingInfo in _waitingItemInfoList)
        {
            if (waitingInfo.Value.cacheCommandEnum == _usedItemInfoList[usedItem].cacheCommandEnum)
            {
                waitingItem = waitingInfo.Key;
                waitingItemInfo = waitingInfo.Value;
                break;
            }
        }

        if (waitingItem == null)
        {
            Debug.Log("找不到对应的waitingItem");
            return;
        }

        waitingItemInfo.cacheCount++;
        callback?.Invoke(waitingItem, waitingItemInfo);
    }

    /// <summary>
    /// waitingItem减一
    /// </summary>
    /// <param name="waitingItem"></param>
    public CommandItemInfo RemoveWaitingCommand(CommandItem waitingItem, Action noWaitingObjCallback)
    {
        CommandItemInfo waitingItemInfo = _waitingItemInfoList[waitingItem];
        waitingItemInfo.cacheCount--;

        if (_waitingItemList.Count <= 0)
        {
            noWaitingObjCallback.Invoke();
        }

        return waitingItemInfo;
    }

    /// <summary>
    /// 检测Index前后的usedItem是否一样，一样则合并
    /// </summary>
    /// <param name="removeUsedItemIndex"></param>
    private void CheckSameAfterRemoveUsedItem(int removeUsedItemIndex, Action<CommandItem, CommandItemInfo> usedRefreshCallback)
    {
        if (_usedItemList.Count < 2 || removeUsedItemIndex <= 0 || removeUsedItemIndex >= _usedItemList.Count)
        {
            return;
        }

        CommandItem lastUsedItem = _usedItemList[removeUsedItemIndex - 1];
        CommandItem currentUsedItem = _usedItemList[removeUsedItemIndex];
        CommandItemInfo lastUsedItemInfo = _usedItemInfoList[lastUsedItem];
        CommandItemInfo currentUsedItemInfo = _usedItemInfoList[currentUsedItem];

        if (lastUsedItemInfo.cacheCommandEnum == currentUsedItemInfo.cacheCommandEnum)
        {
            lastUsedItemInfo.cacheCount += currentUsedItemInfo.cacheCount;
            _usedItemList.Remove(currentUsedItem);
            _usedItemInfoList.Remove(currentUsedItem);
            Destroy(currentUsedItem.gameObject); //对象池处理==================================
            SetUsedItem(lastUsedItem, 0, usedRefreshCallback);
        }
    }

    /// <summary>
    /// 更新usedObj的当前时间
    /// </summary>
    /// <param name="usedItem"></param>
    /// <param name="timeAddon"></param>
    private void SetUsedItem(CommandItem usedItem, int timeAddon, Action<CommandItem, CommandItemInfo> usedRefreshCallback)
    {
        if (_usedItemList.Count == 0)
        {
            return;
        }

        CommandItemInfo usedItemInfo;
        for (int i = _usedItemList.IndexOf(usedItem); i < _usedItemList.Count; i++)
        {
            usedItemInfo = _usedItemInfoList[_usedItemList[i]];
            usedItemInfo.currentTime += timeAddon;
            usedRefreshCallback.Invoke(_usedItemList[i], usedItemInfo);
        }

        CommandItem lastUsedItem = _usedItemList[^1];
        CommandItemInfo lastUsedItemInfo = _usedItemInfoList[lastUsedItem];
        lastUsedItemInfo.currentTime = _currentNeedTime;
        usedRefreshCallback.Invoke(lastUsedItem, lastUsedItemInfo);

        usedItemInfo = _usedItemInfoList[usedItem];
        usedRefreshCallback.Invoke(usedItem, usedItemInfo);
    }
}