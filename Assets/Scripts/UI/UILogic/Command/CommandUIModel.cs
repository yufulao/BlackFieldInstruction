using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandUIModel : MonoBehaviour
{
    private CommandUICtrl _commandUICtrl;
    private RowCfgStage _rowCfgStage;
    private List<CommandItem> _usedItemList;
    private List<CommandItem> _waitingItemList;
    private Dictionary<CommandItem, CommandItemInfo> _usedItemInfoList;
    private Dictionary<CommandItem, CommandItemInfo> _waitingItemInfoList;

    private int _currentNeedTime;


    /// <summary>
    /// 初始化本关卡所有可选指令
    /// </summary>
    /// <param name="commandUICtrl">controller</param>
    /// <param name="originalItemInfoList">初始指令列表</param>
    /// <param name="rowCfgStage">关卡数据</param>
    /// <returns></returns>
    public void OnInit(CommandUICtrl commandUICtrl, Dictionary<CommandItem, CommandItemInfo> originalItemInfoList, RowCfgStage rowCfgStage)
    {
        _commandUICtrl = commandUICtrl;
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

    public CommandItemInfo GetCommandInfo(CommandItem item,bool isUsedItemInfo)
    {
        if (isUsedItemInfo)
        {
            return _usedItemInfoList[item];
        }

        return _waitingItemInfoList[item];
    }

    /// <summary>
    /// usedItem加一
    /// </summary>
    /// <param name="waitingItem"></param>
    public void AddUsedCommand(CommandItem waitingItem)
    {
        CommandItemInfo waitingItemInfo = _waitingItemInfoList[waitingItem];
        _currentNeedTime += waitingItemInfo.cacheTime;
        _commandUICtrl.RefreshCurrentTimeText(_currentNeedTime);

        CommandItem usedItem = null;
        CommandItemInfo usedItemInfo = null;
        if (_usedItemList.Count == 0 || _usedItemInfoList[_usedItemList[^1]].cacheCommandEnum != waitingItemInfo.cacheCommandEnum)
        {
            _commandUICtrl.CreateUsedItem(waitingItemInfo, (item, info) =>
            {
                usedItem = item;
                usedItemInfo = info;
            });
            _usedItemList.Add(usedItem);
            _usedItemInfoList.Add(usedItem,usedItemInfo);
            SetUsedItem(usedItem, waitingItemInfo.cacheTime);
            return;
        }

        //最新的usedObj是同类command
        usedItem = _usedItemList[^1];
        _usedItemInfoList[usedItem].cacheCount++;
        SetUsedItem(usedItem, waitingItemInfo.cacheTime);
    }

    /// <summary>
    /// usedItem减一
    /// </summary>
    /// <param name="usedItem"></param>
    public void RemoveUsedCommand(CommandItem usedItem)
    {
        CommandItemInfo usedItemInfo = _usedItemInfoList[usedItem];
        int timeAddon = -usedItemInfo.cacheTime;
        _currentNeedTime += timeAddon;
        _commandUICtrl.RefreshCurrentTimeText(_currentNeedTime);
        usedItemInfo.cacheCount--;
        SetUsedItem(usedItem, timeAddon);

        if (usedItemInfo.cacheCount <= 0)
        {
            int removeUsedItemIndex = _usedItemList.IndexOf(usedItem);
            _usedItemList.RemoveAt(removeUsedItemIndex);
            Destroy(usedItem.gameObject); //对象池处理==================================
            CheckSameAfterRemoveUsedItem(removeUsedItemIndex);
        }

        if (_usedItemList.Count <= 0)
        {
            _commandUICtrl.NoUsedObj();
        }
    }

    /// <summary>
    /// waitingItem加1
    /// </summary>
    /// <param name="usedItem"></param>
    public void AddWaitingCommand(CommandItem usedItem)
    {
        CommandItem waitingItem = null;
        CommandItemInfo waitingItemInfo = null;
        foreach (var waitingInfo in _waitingItemInfoList)
        {
            if (waitingInfo.Value.cacheCommandEnum==_usedItemInfoList[usedItem].cacheCommandEnum)
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
        waitingItem.Refresh(waitingItemInfo.cacheCount,waitingItemInfo.cacheTime);
    }

    /// <summary>
    /// waitingItem减一
    /// </summary>
    /// <param name="waitingItem"></param>
    public void RemoveWaitingCommand(CommandItem waitingItem)
    {
        CommandItemInfo waitingItemInfo =_waitingItemInfoList[waitingItem];
        waitingItemInfo.cacheCount--;

        if (_waitingItemList.Count <= 0)
        {
            _commandUICtrl.NoWaitingObj();
        }

        waitingItem.Refresh(waitingItemInfo.cacheCount,waitingItemInfo.cacheTime);
    }

    /// <summary>
    /// 检测Index前后的usedItem是否一样，一样则合并
    /// </summary>
    /// <param name="removeUsedItemIndex"></param>
    private void CheckSameAfterRemoveUsedItem(int removeUsedItemIndex)
    {
        if (_usedItemList.Count < 2 || removeUsedItemIndex <= 0 || removeUsedItemIndex >= _usedItemList.Count)
        {
            return;
        }

        CommandItem lastUsedItem = _usedItemList[removeUsedItemIndex - 1];
        CommandItem currentUsedItem = _usedItemList[removeUsedItemIndex];
        CommandItemInfo lastUsedItemInfo = _usedItemInfoList[lastUsedItem];
        CommandItemInfo currentUsedItemInfo=_usedItemInfoList[currentUsedItem];
        
        if (lastUsedItemInfo.cacheCommandEnum == currentUsedItemInfo.cacheCommandEnum)
        {
            lastUsedItemInfo.cacheCount += currentUsedItemInfo.cacheCount;
            _usedItemList.Remove(currentUsedItem);
            _usedItemInfoList.Remove(currentUsedItem);
            Destroy(currentUsedItem.gameObject); //对象池处理==================================
            SetUsedItem(lastUsedItem, 0);
        }
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

        CommandItemInfo usedItemInfo;
        for (int i = _usedItemList.IndexOf(usedItem); i < _usedItemList.Count; i++)
        {
            usedItemInfo = _usedItemInfoList[_usedItemList[i]];
            usedItemInfo.currentTime += timeAddon;
            _usedItemList[i].Refresh(usedItemInfo.cacheCount,usedItemInfo.currentTime);
        }

        CommandItem lastUsedItem = _usedItemList[^1];
        CommandItemInfo lastUsedItemInfo = _usedItemInfoList[lastUsedItem];
        lastUsedItemInfo.currentTime = _currentNeedTime;
        lastUsedItem.Refresh(lastUsedItemInfo.cacheCount,lastUsedItemInfo.currentTime);

        usedItemInfo = _usedItemInfoList[usedItem];
        usedItem.Refresh(usedItemInfo.cacheCount,usedItemInfo.currentTime);
    }
}