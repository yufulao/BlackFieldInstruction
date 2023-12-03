using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandViewCtrl : MonoSingleton<CommandViewCtrl>, UICtrlBase
{
    public CommandViewView _view;
    private CommandViewModel _model;

    public void OnInit(Transform viewRoot, params object[] param)
    {
        _view = new CommandViewView();
        _model = GetComponent<CommandViewModel>();

        _view.OnInit(viewRoot);
        ReloadCommandModel(param);

        BindEvent();
    }

    public void BindEvent()
    {
        _view.startBtn.onClick.AddListener(() => { BattleManager.Instance.ChangeToCommandExcuteState(); });
    }

    public void OpenRoot()
    {
        _view.root.gameObject.SetActive(true);
    }

    public void CloseRoot()
    {
        _view.root.gameObject.SetActive(false);
    }

    public List<UsedCommandObj> GetAllUsedObj()
    {
        return _model.GetUsedObjList();
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
        List<WaitingCommandObj> originalObjCommandList = new List<WaitingCommandObj>();
        foreach (var pair in commandDic)
        {
            GameObject obj = Instantiate(
                AssetManager.Instance.LoadAsset<GameObject>(
                    ConfigManager.Instance.cfgPrefab["WaitingCommandObj"].prefabPath), _view.waitingObjContainer);
            WaitingCommandObj waitingObjTemp = obj.GetComponent<WaitingCommandObj>();
            CommandType commandEnum = (CommandType) pair.Key;
            int needTime = ConfigManager.Instance.cfgCommand[commandEnum.ToString()].needTime;
            if (commandTime.ContainsKey(pair.Key)) //有设置这个指令的needTime，否则用默认needTime
            {
                needTime = commandTime[pair.Key];
            }

            waitingObjTemp.Init(commandEnum, pair.Value, needTime);
            for (int i = 0; i < waitingObjTemp.count; i++)
            {
                CreatWaitingBtn(waitingObjTemp);
            }

            UpdateWaitingObjView(waitingObjTemp);
            originalObjCommandList.Add(waitingObjTemp);
        }

        _model.OnInit(originalObjCommandList, rowCfgStage);
        UpdateCurrentTimeText(0, rowCfgStage.stageTime);
    }

    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingObj"></param>
    public void ClickWaitingObj(WaitingCommandObj waitingObj)
    {
        _model.UpdateCurrentNeedTime(waitingObj.needTime);
        _model.RemoveWaitingCommand(waitingObj);
        _model.AddUsedCommand(waitingObj, _view.usedObjContainer);
    }

    /// <summary>
    /// 点击usedObj
    /// </summary>
    /// <param name="usedObj"></param>
    public void ClickUsedObj(UsedCommandObj usedObj)
    {
        WaitingCommandObj waitingObj = _model.AddWaitingCommand(usedObj.commandEnum);
        _model.UpdateCurrentNeedTime(-waitingObj.needTime);
        _model.RemoveUsedCommand(usedObj);
    }

    public void CreatUsedBtn(UsedCommandObj usedObj)
    {
        GameObject btn = Instantiate(usedObj.clickBtnProto, usedObj.clickBtnContainer);
        btn.transform.Find("Text (Legacy)").GetComponent<Text>().text = usedObj.commandEnum.ToString();
        btn.transform.GetComponent<Button>().onClick.AddListener(() => { ClickUsedObj(usedObj); });
        btn.transform.GetComponent<UIDragComponent>()
            .InitDragComponent(_view.root, usedObj.DragFilter, () => { UsedBtnOnBeginDrag(usedObj);}, usedObj.OnCommandObjEndDragCallback,UsedCommandBtnInvalidDrag);
        usedObj.btnList.Add(btn);
    }

    public void CreatWaitingBtn(WaitingCommandObj waitingObj)
    {
        GameObject btn = Instantiate(waitingObj.clickBtnProto, waitingObj.clickBtnContainer);
        btn.transform.Find("Text (Legacy)").GetComponent<Text>().text = waitingObj.commandEnum.ToString();
        btn.transform.GetComponent<Button>().onClick.AddListener(() => { ClickWaitingObj(waitingObj); });
        btn.transform.GetComponent<UIDragComponent>().InitDragComponent(_view.root, waitingObj.DragFilter,null,
            waitingObj.OnCommandObjEndDragCallback);
        waitingObj.btnList.Add(btn);
    }

    public void UsedBtnOnBeginDrag(UsedCommandObj usedObj)
    {
        if (usedObj.count<=1)
        {
            usedObj.canvasGroup.alpha = 0;
        }
    }

    public void UsedBtnOnEndDragFail(UsedCommandObj usedObj)
    {
        if (usedObj.count<=1)
        {
            usedObj.canvasGroup.alpha = 1;
        }
    }

    /// <summary>
    /// 更新单个可选指令的显示
    /// </summary>
    /// <param name="waitingObj"></param>
    public void UpdateWaitingObjView(WaitingCommandObj waitingObj)
    {
        SetWaitingObjClickBtnMaskActive(waitingObj, waitingObj.count > 0); //当waitingObj的count为0时，关闭waitingObj，否则启用
        waitingObj.countText.text = "x" + waitingObj.count.ToString();
        waitingObj.needTimeText.text = waitingObj.needTime.ToString() + "s";
    }

    /// <summary>
    /// 更新最后一个输入的指令的显示
    /// </summary>
    /// <param name="usedObj"></param>
    public void UpdateLastUsedObjView(UsedCommandObj usedObj)
    {
        usedObj.countText.text = "x" + usedObj.count.ToString();
        usedObj.currentTimeText.text = usedObj.currentTime.ToString() + "s";
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
            _view.stageTimeText.text = "/" + stageTime.ToString() + "s";
        }

        _view.currentTimeText.text = currentTime.ToString();
    }

    /// <summary>
    /// 当waitingObj的count为0时，开启btn的遮罩阻挡点击
    /// </summary>
    /// <param name="waitingObj"></param>
    /// <param name="canClick"></param>
    private void SetWaitingObjClickBtnMaskActive(WaitingCommandObj waitingObj, bool canClick)
    {
        waitingObj.transform.Find("ClickBtnMask").gameObject.SetActive(!canClick);
    }
    
    private void UsedCommandBtnInvalidDrag(PointerEventData eventData)
    {
        //Debug.Log("滑动UsedScroll");
        _view.usedCommandScroll.OnDrag(eventData);
    }
    public void WaitingCommandBtnInvalidDrag(PointerEventData eventData)
    {
        //Debug.Log("滑动WaitingScroll");
        //Debug.Log(distance);
        //_view.waitingCommandScroll.content.anchoredPosition = new Vector2(eventData.position.x,0f );
        // Debug.Log(eventData.delta.y);
        // Debug.Log(_view.waitingCommandScroll.content.anchoredPosition);
    }

    private void Update()
    {
        //Debug.Log(_view.waitingCommandScroll.normalizedPosition);
        //_view.waitingCommandScroll.normalizedPosition = new Vector2(1, 0);
    }
}