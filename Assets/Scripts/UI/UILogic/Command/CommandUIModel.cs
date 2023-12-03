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
    /// 更新当前所需总时间
    /// </summary>
    /// <param name="addTime"></param>
    public void UpdateCurrentNeedTime(int addTime)
    {
        _currentNeedTime += addTime;
        CommandUICtrl.Instance.UpdateCurrentTimeText(_currentNeedTime);
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
    /// <param name="usedItemContainer"></param>
    public void AddUsedCommand(WaitingCommandItem waitingItem, Transform usedItemContainer)
    {
        UsedCommandItem usedItem = null;
        if (_usedItemList.Count == 0 || _usedItemList[^1].commandEnum != waitingItem.commandEnum)
        {
            usedItem =
                Instantiate(
                    AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["UsedCommandItem"]
                        .prefabPath), usedItemContainer).GetComponent<UsedCommandItem>();
            usedItem.Init(waitingItem.commandEnum, 1, waitingItem.needTime);
            CommandUICtrl.Instance.CreatWaitingBtn(usedItem);
            _usedItemList.Add(usedItem);
            UpdateUsedItem(usedItem);
            return;
        }

        //最新的usedObj是同类command
        usedItem = _usedItemList[^1];
        usedItem.count++;
        if (usedItem.btnList.Count > usedItem.count)
        {
            usedItem.btnList[usedItem.count].SetActive(true);
        }
        else
        {
            CommandUICtrl.Instance.CreatWaitingBtn(usedItem);
        }

        UpdateUsedItem(usedItem);
    }

    /// <summary>
    /// waitingItem加1
    /// </summary>
    /// <param name="commandEnum"></param>
    public WaitingCommandItem AddWaitingCommand(CommandType commandEnum)
    {
        WaitingCommandItem waitingItem = null;
        foreach (var waitingItemTemp in _waitingItemList)
        {
            if (waitingItemTemp.commandEnum == commandEnum)
            {
                waitingItem = waitingItemTemp;
                break;
            }
        }

        if (waitingItem == null)
        {
            Debug.Log("找不到对应的waitingItem");
            return null;
        }

        waitingItem.count++;
        waitingItem.btnList[waitingItem.count - 1].SetActive(true);

        CommandUICtrl.Instance.UpdateWaitingObjView(waitingItem);
        return waitingItem;
    }

    /// <summary>
    /// waitingItem减一
    /// </summary>
    /// <param name="waitingItem"></param>
    public void RemoveWaitingCommand(WaitingCommandItem waitingItem)
    {
        waitingItem.count--;
        waitingItem.btnList[waitingItem.count].gameObject.SetActive(false);
        if (_waitingItemList.Count <= 0)
        {
            CommandUICtrl.Instance.NoWaitingObj();
        }

        CommandUICtrl.Instance.UpdateWaitingObjView(waitingItem);
    }

    /// <summary>
    /// usedItem减一
    /// </summary>
    /// <param name="usedItem"></param>
    public void RemoveUsedCommand(UsedCommandItem usedItem)
    {
        usedItem.count--;
        usedItem.btnList[usedItem.count].gameObject.SetActive(false);

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

        UpdateUsedItem(usedItem);
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
            for (int i = 0; i < nextUsedItem.btnList.Count; i++)
            {
                nextUsedItem.btnList[i].transform.SetParent(lastUsedItem.clickBtnContainer);
                nextUsedItem.btnList[i].transform.position = Vector3.zero;
                nextUsedItem.btnList[i].GetComponent<Button>().onClick.RemoveAllListeners();
                nextUsedItem.btnList[i].GetComponent<Button>().onClick.AddListener(() => { CommandUICtrl.Instance.ClickUsedObj(lastUsedItem); });
                lastUsedItem.btnList.Add(nextUsedItem.btnList[i]);
            }

            lastUsedItem.count += nextUsedItem.count;
            _usedItemList.Remove(nextUsedItem);
            Destroy(nextUsedItem.gameObject); //对象池处理==================================
            UpdateUsedItem(lastUsedItem);
        }
    }

    /// <summary>
    /// 更新最新的usedObj的当前时间
    /// </summary>
    /// <param name="usedCommandItem"></param>
    private void UpdateUsedItem(UsedCommandItem usedCommandItem)
    {
        if (_usedItemList.Count == 0)
        {
            return;
        }

        UsedCommandItem lastUsedItem = _usedItemList[^1];
        lastUsedItem.currentTime = _currentNeedTime;
        CommandUICtrl.Instance.UpdateLastUsedObjView(lastUsedItem);
        CommandUICtrl.Instance.UpdateLastUsedObjView(usedCommandItem);
    }
}