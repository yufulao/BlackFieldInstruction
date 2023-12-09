using System;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class CommandUIModel
{
    private List<UsedItemInfo> _usedItemInfoList;
    private List<WaitingItemInfo> _waitingItemInfoList;

    private int _currentNeedTime;


    /// <summary>
    /// 初始化本关卡所有可选指令
    /// </summary>
    /// <param name="originalItemInfoList">初始指令列表</param>
    /// <returns></returns>
    public void OnInit(List<WaitingItemInfo> originalItemInfoList)
    {
        _usedItemInfoList = new List<UsedItemInfo>();
        _currentNeedTime = 0;
        _waitingItemInfoList = originalItemInfoList;
    }

    public void ResetModel(List<WaitingItemInfo> originalItemInfoList)
    {
        _usedItemInfoList.Clear();
        _currentNeedTime = 0;
        _waitingItemInfoList = originalItemInfoList;
    }

    /// <summary>
    /// 获取usedCommandList
    /// </summary>
    /// <returns></returns>
    public List<UsedItemInfo> GetUsedItemList()
    {
        List<UsedItemInfo> usedItemInfoList = new List<UsedItemInfo>();
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
    /// 生成一个UsedItemInfo
    /// </summary>
    /// <param name="waitingItemInfo"></param>
    /// <returns></returns>
    public UsedItemInfo CreatUsedItemInfo(WaitingItemInfo waitingItemInfo)
    {
        return new UsedItemInfo()
        {
            cacheCommandEnum = waitingItemInfo.cacheCommandEnum,
            cacheCount = 1,
            cacheTime = waitingItemInfo.cacheTime,
            currentTime = GetCurrentNeedTime()
        };
    }
    
    /// <summary>
    /// 生成一个waitingItemInfo
    /// </summary>
    /// <param name="commandEnum"></param>
    /// <param name="needTime"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public WaitingItemInfo CreatWaitingItemInfo(CommandType commandEnum, int needTime, int count)
    {
        return new WaitingItemInfo()
        {
            cacheCommandEnum = commandEnum,
            cacheCount = count,
            cacheTime = needTime
        };
    }
    
    /// <summary>
    /// 尝试添加usedList末尾相同的ItemInfo
    /// </summary>
    /// <param name="waitingItemInfo"></param>
    /// <returns></returns>
    public bool TryAddSameUsedCommand(WaitingItemInfo waitingItemInfo)
    {
        if (_usedItemInfoList.Count != 0 && _usedItemInfoList[^1].cacheCommandEnum == waitingItemInfo.cacheCommandEnum)
        {
            //最新的usedObj是同类command
            _currentNeedTime += waitingItemInfo.cacheTime;
            UsedItemInfo usedItemInfo = _usedItemInfoList[^1];
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
    public void AddUsedCommand(WaitingItemInfo waitingItemInfo, UsedItemInfo usedItemInfo)
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
    public void RemoveUsedCommand(UsedItemInfo usedItemInfo, Action noUsedObjCallback)
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
    public WaitingItemInfo AddWaitingCommand(UsedItemInfo usedItemInfo)
    {
        WaitingItemInfo waitingItemInfo = null;
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
    public void RemoveWaitingCommand(WaitingItemInfo waitingItemInfo, Action noWaitingObjCallback)
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
    public void RemoveUsedItemInfo(UsedItemInfo usedItemInfo)
    {
        _usedItemInfoList.Remove(usedItemInfo);
    }

    /// <summary>
    /// 检测Index前后的usedItem是否一样，一样则合并
    /// </summary>
    /// <param name="removeUsedItemIndex"></param>
    private void CombineSameUsedItem(int removeUsedItemIndex)
    {
        if (_usedItemInfoList.Count < 3 || removeUsedItemIndex == 0 || removeUsedItemIndex >= _usedItemInfoList.Count)
        {
            return;
        }

        UsedItemInfo lastUsedItemInfo = _usedItemInfoList[removeUsedItemIndex - 1];
        UsedItemInfo nextUsedItemInfo=_usedItemInfoList[removeUsedItemIndex+1];

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
    private void SetUsedItem(UsedItemInfo usedItemInfo, int timeAddon)
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