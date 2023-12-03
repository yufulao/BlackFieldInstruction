using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandUICtrl : MonoSingleton<CommandUICtrl>, UICtrlBase
{
    private CommandUIModel _model;

    //ui
    private Transform _usedObjContainer;
    private Transform _waitingObjContainer;
    private Button _startBtn;
    private Text _currentTimeText;
    private Text _stageTimeText;
    private ScrollRect _waitingScroll;
    private ScrollRect _usedScroll;

    public void OnInit(params object[] param)
    {
        _model = GetComponent<CommandUIModel>();
        InitView();
        ReloadCommandModel(param);
        BindEvent();
    }

    public void BindEvent()
    {
        _startBtn.onClick.AddListener(() => { BattleManager.Instance.ChangeToCommandExcuteState(); });
    }

    public void OpenRoot()
    {
        gameObject.SetActive(true);
    }

    public void CloseRoot()
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
    /// 重新获取场景中的commandModel和commandView
    /// </summary>
    /// <param name="param"></param>
    private void ReloadCommandModel(params object[] param)
    {
        if (param == null)
        {
            Debug.LogError("没传参数CfgStage");
            return;
        }

        RowCfgStage rowCfgStage = param[0] as RowCfgStage;
        Dictionary<int, int> commandDic = rowCfgStage.commandDic;
        Dictionary<int, int> commandTime = rowCfgStage.commandTime;
        List<WaitingCommandItem> originalObjCommandList = new List<WaitingCommandItem>();
        foreach (var pair in commandDic)
        {
            GameObject waitingObj = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["WaitingCommandItem"].prefabPath)
                , _waitingObjContainer);
            WaitingCommandItem waitingItem = waitingObj.GetComponent<WaitingCommandItem>();
            CommandType commandEnum = (CommandType) pair.Key;
            int needTime = ConfigManager.Instance.cfgCommand[commandEnum.ToString()].needTime;
            if (commandTime.ContainsKey(pair.Key)) //有设置这个指令的needTime，否则用默认needTime
            {
                needTime = commandTime[pair.Key];
            }

            waitingItem.Init(commandEnum, pair.Value, needTime);
            for (int i = 0; i < waitingItem.count; i++)
            {
                CreatWaitingBtn(waitingItem);
            }

            UpdateWaitingObjView(waitingItem);
            originalObjCommandList.Add(waitingItem);
        }

        _model.OnInit(originalObjCommandList, rowCfgStage);
        UpdateCurrentTimeText(0, rowCfgStage.stageTime);
    }

    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingItem"></param>
    public void ClickWaitingObj(WaitingCommandItem waitingItem)
    {
        _model.UpdateCurrentNeedTime(waitingItem.needTime);
        _model.RemoveWaitingCommand(waitingItem);
        _model.AddUsedCommand(waitingItem, _usedObjContainer);
    }

    /// <summary>
    /// 点击usedObj
    /// </summary>
    /// <param name="usedItem"></param>
    public void ClickUsedObj(UsedCommandItem usedItem)
    {
        WaitingCommandItem waitingItem = _model.AddWaitingCommand(usedItem.commandEnum);
        _model.UpdateCurrentNeedTime(-waitingItem.needTime);
        _model.RemoveUsedCommand(usedItem);
    }

    /// <summary>
    /// 创建指令Item的可拖拽的btn
    /// </summary>
    /// <param name="commandItem"></param>
    public void CreatWaitingBtn(CommandItem commandItem)
    {
        if (commandItem as WaitingCommandItem)
        {
            WaitingCommandItem waitingItem = commandItem as WaitingCommandItem;
            GameObject btn = Instantiate(waitingItem.clickBtnProto, waitingItem.clickBtnContainer);
            btn.transform.Find("Text (Legacy)").GetComponent<Text>().text = waitingItem.commandEnum.ToString();
            btn.transform.GetComponent<Button>().onClick.AddListener(() => { ClickWaitingObj(waitingItem); });
            btn.transform.GetComponent<UIDragComponent>().InitDragComponent(transform, waitingItem.DragFilter, null, waitingItem.OnCommandItemEndDragCallback,
                _waitingScroll.OnBeginDrag, _waitingScroll.OnDrag, _waitingScroll.OnEndDrag);
            waitingItem.btnList.Add(btn);
        }

        if (commandItem as UsedCommandItem)
        {
            UsedCommandItem usedItem = commandItem as UsedCommandItem;
            GameObject btn = Instantiate(usedItem.clickBtnProto, usedItem.clickBtnContainer);
            btn.transform.Find("Text (Legacy)").GetComponent<Text>().text = usedItem.commandEnum.ToString();
            btn.transform.GetComponent<Button>().onClick.AddListener(() => { ClickUsedObj(usedItem); });
            btn.transform.GetComponent<UIDragComponent>().InitDragComponent(transform, usedItem.DragFilter, () => { UsedBtnOnBeginDrag(usedItem); },
                usedItem.OnCommandItemEndDragCallback, _usedScroll.OnBeginDrag, _usedScroll.OnDrag, _usedScroll.OnEndDrag);
            usedItem.btnList.Add(btn);
        }
    }

    /// <summary>
    /// 只剩下一个usedBtn开始拖拽的事件，隐藏count和currentTime显示
    /// </summary>
    /// <param name="usedItem"></param>
    public void UsedBtnOnBeginDrag(UsedCommandItem usedItem)
    {
        if (usedItem.count <= 1)
        {
            usedItem.canvasGroup.alpha = 0;
        }
    }

    /// <summary>
    /// 只剩下一个usedBtn拖拽失败的事件，恢复count和currentTime显示
    /// </summary>
    /// <param name="usedItem"></param>
    public void UsedBtnOnEndDragFail(UsedCommandItem usedItem)
    {
        if (usedItem.count <= 1)
        {
            usedItem.canvasGroup.alpha = 1;
        }
    }

    /// <summary>
    /// 更新单个可选指令的显示
    /// </summary>
    /// <param name="waitingItem"></param>
    public void UpdateWaitingObjView(WaitingCommandItem waitingItem)
    {
        SetWaitingObjClickBtnMaskActive(waitingItem, waitingItem.count > 0); //当waitingObj的count为0时，关闭waitingObj，否则启用
        waitingItem.countText.text = "x" + waitingItem.count.ToString();
        waitingItem.needTimeText.text = waitingItem.needTime.ToString() + "s";
    }

    /// <summary>
    /// 更新最后一个输入的指令的显示
    /// </summary>
    /// <param name="usedItem"></param>
    public void UpdateLastUsedObjView(UsedCommandItem usedItem)
    {
        usedItem.countText.text = "x" + usedItem.count.ToString();
        usedItem.currentTimeText.text = usedItem.currentTime.ToString() + "s";
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
    public void UpdateCurrentTimeText(int currentTime, int stageTime = -1)
    {
        if (stageTime != -1)
        {
            _stageTimeText.text = "/" + stageTime.ToString() + "s";
        }

        _currentTimeText.text = currentTime.ToString();
    }

    /// <summary>
    /// 当waitingObj的count为0时，开启btn的遮罩阻挡点击
    /// </summary>
    /// <param name="waitingItem"></param>
    /// <param name="canClick"></param>
    private void SetWaitingObjClickBtnMaskActive(WaitingCommandItem waitingItem, bool canClick)
    {
        waitingItem.transform.Find("ClickBtnMask").gameObject.SetActive(!canClick);
    }

    private void InitView()
    {
        Transform downFrame = transform.Find("DownFrame");
        Transform upFrame = transform.Find("UpFrame");

        _usedObjContainer = downFrame.Find("UsedCommandItemList").Find("Viewport").Find("UsedItemContainer");
        _waitingObjContainer = downFrame.Find("WaitingCommandItemList").Find("Viewport").Find("WaitingItemContainer");
        _waitingScroll = downFrame.Find("WaitingCommandItemList").GetComponent<ScrollRect>();
        _usedScroll = downFrame.Find("UsedCommandItemList").GetComponent<ScrollRect>();

        _startBtn = upFrame.Find("StartBtn").GetComponent<Button>();
        _currentTimeText = upFrame.Find("TimeShow").Find("CurrentTimeText").GetComponent<Text>();
        _stageTimeText = upFrame.Find("TimeShow").Find("StageTimeText").GetComponent<Text>();
    }
}