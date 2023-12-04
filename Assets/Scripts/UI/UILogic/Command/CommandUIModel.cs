using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandUIModel : MonoBehaviour
{
    private RowCfgStage _rowCfgStage;
    private List<UsedCommandItem> _usedItemList;
    private List<WaitingCommandItem> _waitingItemList;

    private int _currentNeedTime;


    /// <summary>
    /// 初始化本关卡所有可选指令
    /// </summary>
    /// <param name="originalCommandItemList">初始指令列表</param>
    /// <param name="rowCfgStage">关卡数据</param>
    /// <returns></returns>
    public void OnInit(List<WaitingCommandItem> originalCommandItemList, RowCfgStage rowCfgStage)
    {
        _rowCfgStage = rowCfgStage;
        _usedItemList = new List<UsedCommandItem>();
        _waitingItemList = new List<WaitingCommandItem>();
        _currentNeedTime = 0;
        _waitingItemList = originalCommandItemList;
    }

    /// <summary>
    /// 获取usedCommandList
    /// </summary>
    /// <returns></returns>
    public List<UsedCommandItem> GetUsedItemList()
    {
        return _usedItemList;
    }

    /// <summary>
    /// usedItem加一
    /// </summary>
    /// <param name="waitingItem"></param>
    public void AddUsedCommand(WaitingCommandItem waitingItem)
    {
        _currentNeedTime += waitingItem.needTime;
        CommandUICtrl.Instance.UpdateCurrentTimeText(_currentNeedTime);

        UsedCommandItem usedItem = null;
        if (_usedItemList.Count == 0 || _usedItemList[^1].commandEnum != waitingItem.commandEnum)
        {
            usedItem = CommandUICtrl.Instance.AddUsedItem(waitingItem);
            _usedItemList.Add(usedItem);
            UpdateUsedItem(usedItem, waitingItem.needTime);
            return;
        }

        //最新的usedObj是同类command
        usedItem = _usedItemList[^1];
        usedItem.count++;
        UpdateUsedItem(usedItem, waitingItem.needTime);
    }

    /// <summary>
    /// usedItem减一
    /// </summary>
    /// <param name="usedItem"></param>
    /// <param name="timeAddon"></param>
    public void RemoveUsedCommand(UsedCommandItem usedItem, int timeAddon)
    {
        _currentNeedTime += timeAddon;
        CommandUICtrl.Instance.UpdateCurrentTimeText(_currentNeedTime);

        usedItem.count--;
        if (usedItem.count <= 0)
        {
            int removeUsedItemIndex = _usedItemList.IndexOf(usedItem);
            _usedItemList.RemoveAt(removeUsedItemIndex);
            Destroy(usedItem.gameObject); //对象池处理==================================
            CheckSameAfterRemoveUsedItem(removeUsedItemIndex);
        }

        if (_usedItemList.Count <= 0)
        {
            CommandUICtrl.Instance.NoUsedObj();
        }

        UpdateUsedItem(usedItem,timeAddon);
    }

    /// <summary>
    /// waitingItem加1
    /// </summary>
    /// <param name="usedItem"></param>
    public void AddWaitingCommand(UsedCommandItem usedItem)
    {
        WaitingCommandItem waitingItem = null;
        foreach (var waitingItemTemp in _waitingItemList)
        {
            if (waitingItemTemp.commandEnum == usedItem.commandEnum)
            {
                waitingItem = waitingItemTemp;
                break;
            }
        }

        if (waitingItem == null)
        {
            Debug.Log("找不到对应的waitingItem");
            return;
        }

        waitingItem.count++;
        waitingItem.UpdateView();
    }

    /// <summary>
    /// waitingItem减一
    /// </summary>
    /// <param name="waitingItem"></param>
    public void RemoveWaitingCommand(WaitingCommandItem waitingItem)
    {
        waitingItem.count--;

        if (_waitingItemList.Count <= 0)
        {
            CommandUICtrl.Instance.NoWaitingObj();
        }

        waitingItem.UpdateView();
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

        if (_usedItemList[removeUsedItemIndex - 1].commandEnum == _usedItemList[removeUsedItemIndex].commandEnum)
        {
            var lastUsedItem = _usedItemList[removeUsedItemIndex - 1];
            var nextUsedItem = _usedItemList[removeUsedItemIndex];

            lastUsedItem.count += nextUsedItem.count;
            _usedItemList.Remove(nextUsedItem);
            Destroy(nextUsedItem.gameObject); //对象池处理==================================
            UpdateUsedItem(lastUsedItem,0);
        }
    }

    /// <summary>
    /// 更新最新的usedObj的当前时间
    /// </summary>
    /// <param name="usedCommandItem"></param>
    /// <param name="timeAddon"></param>
    private void UpdateUsedItem(UsedCommandItem usedCommandItem, int timeAddon)
    {
        if (_usedItemList.Count == 0)
        {
            return;
        }

        for (int i = _usedItemList.IndexOf(usedCommandItem) + 1; i < _usedItemList.Count; i++)
        {
            _usedItemList[i].currentTime += timeAddon;
            _usedItemList[i].UpdateLastUsedObjView();
        }

        UsedCommandItem lastUsedItem = _usedItemList[^1];
        lastUsedItem.currentTime = _currentNeedTime;
        lastUsedItem.UpdateLastUsedObjView();

        usedCommandItem.UpdateLastUsedObjView();
    }
}