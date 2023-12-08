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

    private readonly Dictionary<UsedItemInfo, CommandItem> _usedItemInfoDic = new Dictionary<UsedItemInfo, CommandItem>();
    private readonly Dictionary<WaitingItemInfo, CommandItem> _waitingItemInfoDic = new Dictionary<WaitingItemInfo, CommandItem>();

    public override void OnInit(params object[] param)
    {
        _model = new CommandUIModel();
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
    public List<UsedItemInfo> GetAllUsedItem()
    {
        return _model.GetUsedItemList();
    }

    protected override void BindEvent()
    {
        startBtn.onClick.AddListener(() => BattleManager.Instance.ChangeToCommandExcuteState());
    }

    /// <summary>
    /// 没有可选指令时
    /// </summary>
    private void NoWaitingObj()
    {
    }

    /// <summary>
    /// 没有输入指令时
    /// </summary>
    private void NoUsedObj()
    {
    }

    /// <summary>
    /// 更新上方显示的的当前时间text
    /// </summary>
    private void RefreshCurrentTimeText(int currentTime, int stageTime = -1)
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
    /// <param name="waitingItemInfo"></param>
    /// <returns></returns>
    private UsedItemInfo CreateUsedItem(WaitingItemInfo waitingItemInfo)
    {
        CommandItem usedItem = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["CommandItem"].prefabPath)
            , usedItemContainer).GetComponent<CommandItem>();
        UsedItemInfo usedItemInfo = _model.CreatUsedItemInfo(waitingItemInfo);
        usedItem.Init(usedItemInfo.cacheCommandEnum, transform);
        usedItem.SetBtnOnClick(UsedItemOnClick);
        usedItem.SetDragAction(UsedItemDragFilter, UsedBtnOnBeginDrag, UsedItemOnEndDrag);
        usedItem.SetInvalidDragAction(usedScroll.OnBeginDrag, usedScroll.OnDrag, usedScroll.OnEndDrag);
        _usedItemInfoDic.Add(usedItemInfo, usedItem);
        return usedItemInfo;
    }

    /// <summary>
    /// 生成一个WaitingItem
    /// </summary>
    /// <param name="commandEnum"></param>
    /// <param name="needTime"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private void CreateWaitingItem(CommandType commandEnum, int needTime, int count, Action<CommandItem, WaitingItemInfo> callback)
    {
        CommandItem item = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["CommandItem"].prefabPath)
            , waitingItemContainer).GetComponent<CommandItem>();
        WaitingItemInfo info = _model.CreatWaitingItemInfo(commandEnum,needTime,count);
        item.Init(commandEnum, transform);
        item.SetBtnOnClick(WaitingItemOnClick);
        item.SetDragAction(WaitingItemDragFilter, null, WaitingItemOnEndDrag);
        item.SetInvalidDragAction(waitingScroll.OnBeginDrag, waitingScroll.OnDrag, waitingScroll.OnEndDrag);

        callback?.Invoke(item, info);
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

        _waitingItemInfoDic.Clear();

        RowCfgStage rowCfgStage = param[0] as RowCfgStage;
        Dictionary<int, int> commandDic = rowCfgStage.commandDic;
        Dictionary<int, int> commandTime = rowCfgStage.commandTime;
        List<WaitingItemInfo> originalItemList = new List<WaitingItemInfo>();
        foreach (var pair in commandDic)
        {
            CommandType commandEnum = (CommandType) pair.Key;
            int needTime = ConfigManager.Instance.cfgCommand[commandEnum.ToString()].needTime; //默认值
            if (commandTime.ContainsKey(pair.Key)) //有设置这个指令的needTime，否则用默认needTime
            {
                needTime = commandTime[pair.Key];
            }

            CreateWaitingItem(commandEnum, needTime, pair.Value, (item, info) =>
            {
                item.Refresh(info.cacheCount, info.cacheTime);
                originalItemList.Add(info);
                _waitingItemInfoDic.Add(info, item);
            });
        }

        _model.OnInit(originalItemList);
        RefreshCurrentTimeText(0, rowCfgStage.stageTime);
    }

    /// <summary>
    /// 刷新整个usedItem列表
    /// </summary>
    private void RefreshUsedItemList()
    {
        List<UsedItemInfo> needRefreshUsedItemInfos = _model.GetUsedItemList();
        if (needRefreshUsedItemInfos == null)
        {
            return;
        }

        for (int i = 0; i < needRefreshUsedItemInfos.Count; i++)
        {
            CommandItem usedItem = _usedItemInfoDic[needRefreshUsedItemInfos[i]];
            if (needRefreshUsedItemInfos[i].cacheCount <= 0)
            {
                _usedItemInfoDic.Remove(needRefreshUsedItemInfos[i]);
                _model.RemoveUsedItemInfo(needRefreshUsedItemInfos[i]);
                Destroy(usedItem.gameObject); //对象池解决======================================================================
            }

            usedItem.Refresh(needRefreshUsedItemInfos[i].cacheCount, needRefreshUsedItemInfos[i].currentTime);
        }
    }

    /// <summary>
    /// 点击usedObj
    /// </summary>
    /// <param name="usedItem"></param>
    private void UsedItemOnClick(CommandItem usedItem)
    {
        UsedItemInfo usedItemInfo = GetUsedInfoByItem(usedItem);
        WaitingItemInfo waitingItemInfo = _model.AddWaitingCommand(usedItemInfo);
        _waitingItemInfoDic[waitingItemInfo].Refresh(waitingItemInfo.cacheCount, waitingItemInfo.cacheTime);
        _model.RemoveUsedCommand(usedItemInfo, NoUsedObj);

        RefreshUsedItemList();
        RefreshCurrentTimeText(_model.GetCurrentNeedTime());
    }

    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingItem"></param>
    private void WaitingItemOnClick(CommandItem waitingItem)
    {
        WaitingItemInfo waitingItemInfo = GetWaitingInfoByItem(waitingItem);
        _model.RemoveWaitingCommand(waitingItemInfo, NoWaitingObj);
        waitingItem.Refresh(waitingItemInfo.cacheCount, waitingItemInfo.cacheTime);
        if (!_model.TryAddSameUsedCommand(waitingItemInfo))
        {
            UsedItemInfo usedItemInfo = CreateUsedItem(waitingItemInfo);
            _model.AddUsedCommand(waitingItemInfo, usedItemInfo);
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
        UsedItemInfo usedItemInfo = GetUsedInfoByItem(usedItem);
        if (resultObjs == null)
        {
            UsedBtnOnEndDragFail(usedItemInfo);
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

        UsedBtnOnEndDragFail(usedItemInfo);
    }

    /// <summary>
    /// 只剩下一个usedBtn拖拽失败的事件，恢复count和currentTime显示
    /// </summary>
    private void UsedBtnOnEndDragFail(UsedItemInfo usedItemInfo)
    {
        if (usedItemInfo.cacheCount <= 1)
        {
            _usedItemInfoDic[usedItemInfo].ShowItem();
        }
    }

    /// <summary>
    /// 只剩下一个usedBtn开始拖拽的事件，隐藏count和currentTime显示
    /// </summary>
    private void UsedBtnOnBeginDrag(CommandItem usedItem)
    {
        if (GetUsedInfoByItem(usedItem).cacheCount <= 1)
        {
            usedItem.HideItem();
        }
    }


    private UsedItemInfo GetUsedInfoByItem(CommandItem item)
    {
        foreach (var pair in _usedItemInfoDic)
        {
            if (pair.Value == item)
            {
                return pair.Key;
            }
        }

        Debug.LogWarning("没有这个item的info" + item);
        return null;
    }
    private WaitingItemInfo GetWaitingInfoByItem(CommandItem item)
    {
        foreach (var pair in _waitingItemInfoDic)
        {
            if (pair.Value == item)
            {
                return pair.Key;
            }
        }

        Debug.LogWarning("没有这个item的info" + item);
        return null;
    }
}