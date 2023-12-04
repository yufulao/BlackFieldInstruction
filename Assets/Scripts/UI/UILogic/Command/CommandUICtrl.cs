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
    public List<UsedCommandItem> GetAllUsedItem()
    {
        return _model.GetUsedItemList();
    }

    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingItem"></param>
    public void ClickWaitingItem(WaitingCommandItem waitingItem)
    {
        _model.RemoveWaitingCommand(waitingItem);
        _model.AddUsedCommand(waitingItem);
    }

    /// <summary>
    /// 点击usedObj
    /// </summary>
    /// <param name="usedItem"></param>
    public void ClickUsedItem(UsedCommandItem usedItem)
    {
        _model.AddWaitingCommand(usedItem);
        _model.RemoveUsedCommand(usedItem, -usedItem.needTime);
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
    /// 生成一个AddUsedItem
    /// </summary>
    /// <param name="waitingItem"></param>
    /// <returns></returns>
    public UsedCommandItem AddUsedItem(WaitingCommandItem waitingItem)
    {
        UsedCommandItem usedItem = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["UsedCommandItem"].prefabPath)
            , usedItemContainer).GetComponent<UsedCommandItem>();
        usedItem.Init(waitingItem.commandEnum, 1, waitingItem.needTime, waitingItem.needTime, transform, () => ClickUsedItem(usedItem)
            , usedScroll.OnBeginDrag, usedScroll.OnDrag, usedScroll.OnEndDrag);
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
        List<WaitingCommandItem> originalWaitingItemList = new List<WaitingCommandItem>();
        foreach (var pair in commandDic)
        {
            CommandType commandEnum = (CommandType) pair.Key;
            WaitingCommandItem waitingItem = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["WaitingCommandItem"].prefabPath)
                , waitingItemContainer).GetComponent<WaitingCommandItem>();
            int needTime = ConfigManager.Instance.cfgCommand[commandEnum.ToString()].needTime; //默认值
            if (commandTime.ContainsKey(pair.Key)) //有设置这个指令的needTime，否则用默认needTime
            {
                needTime = commandTime[pair.Key];
            }

            waitingItem.Init(commandEnum, pair.Value, needTime, transform, () => ClickWaitingItem(waitingItem)
                , waitingScroll.OnBeginDrag, waitingScroll.OnDrag, waitingScroll.OnEndDrag);
            originalWaitingItemList.Add(waitingItem);
        }

        _model.OnInit(this, originalWaitingItemList, rowCfgStage);
        RefreshCurrentTimeText(0, rowCfgStage.stageTime);
    }

}