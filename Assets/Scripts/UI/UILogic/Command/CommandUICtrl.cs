using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandUICtrl : UICtrlBase
{
    private CommandUIModel _model;

    //ui
    [SerializeField] private Button startBtn;
    [SerializeField] private Text currentTimeText;
    [SerializeField] private Text stageTimeText;
    [SerializeField] private ScrollRect usedScroll;
    [SerializeField] private ScrollRect waitingScroll;
    [SerializeField] private Transform usedItemContainer;
    [SerializeField] private Transform waitingItemContainer;

    public override void OnInit(params object[] param)
    {
        _model = GetComponent<CommandUIModel>();
        ReloadModel(param);
        BindEvent();
    }

    public override void OpenRoot()
    {
        gameObject.SetActive(true);
    }

    public override void CloseRoot()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取所有UsedItem
    /// </summary>
    /// <returns></returns>
    public List<CommandItem> GetAllUsedItem()
    {
        return _model.GetUsedItemList();
    }

    /// <summary>
    /// 没有可选指令时
    /// </summary>
    public void NoWaitingObj()
    {
    }

    /// <summary>
    /// 没有输入指令时
    /// </summary>
    public void NoUsedObj()
    {
    }

    /// <summary>
    /// 更新上方显示的的当前时间text
    /// </summary>
    public void RefreshCurrentTimeText(int currentTime, int stageTime = -1)
    {
        if (stageTime != -1)
        {
            stageTimeText.text = "/" + stageTime.ToString() + "s";
        }

        currentTimeText.text = currentTime.ToString();
    }

    /// <summary>
    /// 生成一个UsedItem
    /// </summary>
    /// <param name="waitingItem"></param>
    /// <returns></returns>
    public CommandItem CreateUsedItem(CommandItem waitingItem)
    {
        CommandItem usedItem = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["CommandItem"].prefabPath)
            , usedItemContainer).GetComponent<CommandItem>();
        CommandItemInfo usedItemInfo = new CommandItemInfo()
        {
            cacheCommandEnum = waitingItem.itemInfo.cacheCommandEnum,
            cacheCount = 1,
            cacheTime = waitingItem.itemInfo.cacheTime,
            currentTime = _model.GetCurrentNeedTime()
        };
        usedItem.Init(usedItemInfo.cacheCommandEnum, transform, usedItemInfo);
        usedItem.SetBtnOnClick(UsedItemOnClick);
        usedItem.SetDragAction(UsedItemDragFilter, UsedBtnOnBeginDrag, UsedItemOnEndDrag);
        usedItem.SetValidDragAction(usedScroll.OnBeginDrag, usedScroll.OnDrag, usedScroll.OnEndDrag);
        return usedItem;
    }

    protected override void BindEvent()
    {
        startBtn.onClick.AddListener(() => BattleManager.Instance.ChangeToCommandExcuteState());
    }

    /// <summary>
    /// 重新获取场景中的commandModel和commandView
    /// </summary>
    /// <param name="param">param[0]传关卡数据rowCfgStage</param>
    private void ReloadModel(params object[] param)
    {
        if (param == null)
        {
            Debug.LogError("没传参数CfgStage");
            return;
        }

        RowCfgStage rowCfgStage = param[0] as RowCfgStage;
        Dictionary<int, int> commandDic = rowCfgStage.commandDic;
        Dictionary<int, int> commandTime = rowCfgStage.commandTime;
        List<CommandItem> originalItemList = new List<CommandItem>();
        foreach (var pair in commandDic)
        {
            CommandType commandEnum = (CommandType) pair.Key;
            CommandItem item = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["CommandItem"].prefabPath)
                , waitingItemContainer).GetComponent<CommandItem>();
            int needTime = ConfigManager.Instance.cfgCommand[commandEnum.ToString()].needTime; //默认值
            if (commandTime.ContainsKey(pair.Key)) //有设置这个指令的needTime，否则用默认needTime
            {
                needTime = commandTime[pair.Key];
            }

            CommandItemInfo info = new CommandItemInfo()
            {
                cacheCommandEnum = commandEnum,
                cacheCount = pair.Value,
                cacheTime = needTime
            };

            item.Init(commandEnum, transform, info);
            item.SetBtnOnClick(WaitingItemOnClick);
            item.SetDragAction(WaitingItemDragFilter, null, WaitingItemOnEndDrag);
            item.SetValidDragAction(waitingScroll.OnBeginDrag, waitingScroll.OnDrag, waitingScroll.OnEndDrag);
            item.Refresh(info.cacheCount,info.cacheTime);
            originalItemList.Add(item);
        }

        _model.OnInit(originalItemList, rowCfgStage);
        RefreshCurrentTimeText(0, rowCfgStage.stageTime);
    }

    /// <summary>
    /// 刷新整个usedItem列表
    /// </summary>
    private void RefreshUsedItemList()
    {
        List<CommandItem> needRefreshUsedItems = _model.GetUsedItemList();
        if (needRefreshUsedItems==null)
        {
            return;
        }

        for (int i = 0; i < needRefreshUsedItems.Count; i++)
        {
            needRefreshUsedItems[i].Refresh(needRefreshUsedItems[i].itemInfo.cacheCount,needRefreshUsedItems[i].itemInfo.currentTime);
        }
    }

    /// <summary>
    /// 点击usedObj
    /// </summary>
    /// <param name="usedItem"></param>
    private void UsedItemOnClick(CommandItem usedItem)
    {
        CommandItem waitingItem = _model.AddWaitingCommand(usedItem);
        waitingItem.Refresh(waitingItem.itemInfo.cacheCount, waitingItem.itemInfo.cacheTime);
        _model.RemoveUsedCommand(usedItem, NoUsedObj);

        RefreshUsedItemList();
        RefreshCurrentTimeText(_model.GetCurrentNeedTime());
    }

    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingItem"></param>
    private void WaitingItemOnClick(CommandItem waitingItem)
    {
        _model.RemoveWaitingCommand(waitingItem, NoWaitingObj);
        waitingItem.Refresh(waitingItem.itemInfo.cacheCount, waitingItem.itemInfo.cacheTime);
        bool hasSame= _model.TryAddSameUsedCommand(waitingItem);
        if (!hasSame)
        {
            CommandItem usedItem= CreateUsedItem(waitingItem);
            _model.AddUsedCommand(waitingItem, usedItem);
        }

        RefreshUsedItemList();
        RefreshCurrentTimeText(_model.GetCurrentNeedTime());
    }

    /// <summary>
    /// waitingItem的拖拽过滤器
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool WaitingItemDragFilter(GameObject obj)
    {
        if (obj.name == "UsedCommandItemList")
        {
            return true; //保留
        }

        return false; //移除
    }

    /// <summary>
    /// waitingItem的拖拽结束事件
    /// </summary>
    /// <param name="waitingItem"></param>
    /// <param name="resultObjs"></param>
    private void WaitingItemOnEndDrag(CommandItem waitingItem, List<GameObject> resultObjs)
    {
        if (resultObjs == null)
        {
            return;
        }

        for (int i = 0; i < resultObjs.Count; i++)
        {
            if (resultObjs[i].name == "UsedCommandItemList")
            {
                WaitingItemOnClick(waitingItem);
                break;
            }
        }
    }

    /// <summary>
    /// usedItem的拖拽过滤器
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool UsedItemDragFilter(GameObject obj)
    {
        //Debug.Log(obj.name);
        if (obj.name == "WaitingCommandItemList")
        {
            return true; //保留
        }

        return false; //移除
    }

    /// <summary>
    /// usedItem的拖拽结束事件
    /// </summary>
    /// <param name="usedItem"></param>
    /// <param name="resultObjs"></param>
    private void UsedItemOnEndDrag(CommandItem usedItem, List<GameObject> resultObjs)
    {
        if (resultObjs == null)
        {
            UsedBtnOnEndDragFail(usedItem);
            return;
        }

        for (int i = 0; i < resultObjs.Count; i++)
        {
            if (resultObjs[i].name == "WaitingCommandItemList")
            {
                UsedItemOnClick(usedItem);
                return;
            }
        }

        UsedBtnOnEndDragFail(usedItem);
    }

    /// <summary>
    /// 只剩下一个usedBtn拖拽失败的事件，恢复count和currentTime显示
    /// </summary>
    private void UsedBtnOnEndDragFail(CommandItem usedItem)
    {
        if (usedItem.itemInfo.cacheCount <= 1)
        {
            usedItem.ShowItem();
        }
    }

    /// <summary>
    /// 只剩下一个usedBtn开始拖拽的事件，隐藏count和currentTime显示
    /// </summary>
    private void UsedBtnOnBeginDrag(CommandItem usedItem)
    {
        if (usedItem.itemInfo.cacheCount <= 1)
        {
            usedItem.HideItem();
        }
    }
}