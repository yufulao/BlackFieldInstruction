using System;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class CommandUIModel : MonoBehaviour
{
    private RowCfgStage _rowCfgStage;
    private List<CommandItem> _usedItemList;
    private List<CommandItem> _waitingItemList;

    private int _currentNeedTime;


    /// <summary>
    /// 初始化本关卡所有可选指令
    /// </summary>
    /// <param name="originalItemList">初始指令列表</param>
    /// <param name="rowCfgStage">关卡数据</param>
    /// <returns></returns>
    public void OnInit(List<CommandItem> originalItemList, RowCfgStage rowCfgStage)
    {
        _rowCfgStage = rowCfgStage;
        _usedItemList = new List<CommandItem>();
        _waitingItemList = new List<CommandItem>();
        _currentNeedTime = 0;
        _waitingItemList = originalItemList;
    }

    /// <summary>
    /// 获取usedCommandList
    /// </summary>
    /// <returns></returns>
    public List<CommandItem> GetUsedItemList()
    {
        List<CommandItem> usedItemList = new List<CommandItem>();
        for (int i = 0; i < _usedItemList.Count; i++)
        {
            usedItemList.Add(_usedItemList[i]);
        }

        return usedItemList;
    }

    /// <summary>
    /// 获取当前总时间
    /// </summary>
    /// <returns></returns>
    public int GetCurrentNeedTime()
    {
        return _currentNeedTime;
    }

    public bool TryAddSameUsedCommand(CommandItem waitingItem)
    {
        if (_usedItemList.Count != 0 && _usedItemList[^1].itemInfo.cacheCommandEnum == waitingItem.itemInfo.cacheCommandEnum)
        {
            //最新的usedObj是同类command
            _currentNeedTime += waitingItem.itemInfo.cacheTime;
            CommandItem usedItem = _usedItemList[^1];
            usedItem.itemInfo.cacheCount++;
            SetUsedItem(usedItem, waitingItem.itemInfo.cacheTime);
            return true;
        }

        return false;
    }

    /// <summary>
    /// usedItem加一
    /// </summary>
    /// <param name="waitingItem"></param>
    /// <param name="usedItem"></param>
    public void AddUsedCommand(CommandItem waitingItem, CommandItem usedItem)
    {
        _currentNeedTime += waitingItem.itemInfo.cacheTime;
        _usedItemList.Add(usedItem);
        SetUsedItem(usedItem, waitingItem.itemInfo.cacheTime);
    }

    /// <summary>
    /// usedItem减一
    /// </summary>
    /// <param name="usedItem"></param>
    /// <param name="noUsedObjCallback"></param>
    public void RemoveUsedCommand(CommandItem usedItem, Action noUsedObjCallback)
    {
        int timeAddon = -usedItem.itemInfo.cacheTime;
        _currentNeedTime += timeAddon;
        usedItem.itemInfo.cacheCount--;
        
        int removeUsedItemIndex = _usedItemList.IndexOf(usedItem);
        SetUsedItem(usedItem, timeAddon);
        CombineSameUsedItem(removeUsedItemIndex);

        if (_usedItemList.Count <= 0)
        {
            noUsedObjCallback.Invoke();
        }
    }

    /// <summary>
    /// waitingItem加1
    /// </summary>
    /// <param name="usedItem"></param>
    public CommandItem AddWaitingCommand(CommandItem usedItem)
    {
        CommandItem waitingItem = null;
        for (int i = 0; i < _waitingItemList.Count; i++)
        {
            if (_waitingItemList[i].itemInfo.cacheCommandEnum == usedItem.itemInfo.cacheCommandEnum)
            {
                waitingItem = _waitingItemList[i];
                break;
            }
        }

        if (waitingItem == null)
        {
            Debug.Log("找不到对应的waitingItem");
            return null;
        }

        waitingItem.itemInfo.cacheCount++;
        return waitingItem;
    }

    /// <summary>
    /// waitingItem减一
    /// </summary>
    /// <param name="waitingItem"></param>
    public void RemoveWaitingCommand(CommandItem waitingItem, Action noWaitingObjCallback)
    {
        waitingItem.itemInfo.cacheCount--;

        if (_waitingItemList.Count <= 0)
        {
            noWaitingObjCallback.Invoke();
        }
    }

    /// <summary>
    /// 检测Index前后的usedItem是否一样，一样则合并
    /// </summary>
    /// <param name="removeUsedItemIndex"></param>
    private void CombineSameUsedItem(int removeUsedItemIndex)
    {
        if (_usedItemList.Count < 2 || removeUsedItemIndex <= 0 || removeUsedItemIndex >= _usedItemList.Count)
        {
            return;
        }

        CommandItem lastUsedItem = _usedItemList[removeUsedItemIndex - 1];
        CommandItem nextUsedItem=_usedItemList[removeUsedItemIndex];

        if (lastUsedItem.itemInfo.cacheCommandEnum == nextUsedItem.itemInfo.cacheCommandEnum)
        {
            lastUsedItem.itemInfo.cacheCount += nextUsedItem.itemInfo.cacheCount;
            lastUsedItem.itemInfo.currentTime = nextUsedItem.itemInfo.currentTime;
            _usedItemList.Remove(nextUsedItem);
            Destroy(nextUsedItem.gameObject); //对象池处理==================================
        }

        SetUsedItem(lastUsedItem, 0);
    }

    /// <summary>
    /// 更新usedObj的当前时间
    /// </summary>
    /// <param name="usedItem"></param>
    /// <param name="timeAddon"></param>
    private void SetUsedItem(CommandItem usedItem, int timeAddon)
    {
        if (_usedItemList.Count == 0)
        {
            return;
        }

        for (int i = _usedItemList.IndexOf(usedItem); i < _usedItemList.Count; i++)
        {
            _usedItemList[i].itemInfo.currentTime += timeAddon;
        }
        
        if (usedItem.itemInfo.cacheCount<=0)
        {
            _usedItemList.Remove(usedItem);
            Destroy(usedItem.gameObject);//对象池处理=====================================================
        }
    }
}