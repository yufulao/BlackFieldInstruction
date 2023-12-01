using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandViewCtrl : MonoSingleton<CommandViewCtrl>,UICtrlBase
{
    private CommandViewView _view;
    private CommandViewModel _model;

    public void OnInit(Transform viewRoot, params object[] param)
    {
        _view = new CommandViewView();
        _model=GetComponent<CommandViewModel>();
        
        _view.OnInit(viewRoot);
        ReloadCommandModel(param);
        
        BindEvent();
    }

    public void BindEvent()
    {
        _view.startBtn.onClick.AddListener(()=>{BattleManager.Instance.ChangeToCommandExcuteState();});
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
    /// <param name="rowCfgStage"></param>
    private void ReloadCommandModel(params object[] param)
    {
        if (param==null)
        {
            Debug.LogError("没传参数CfgStage");
            return;
        }
        
        RowCfgStage rowCfgStage = param[0] as RowCfgStage;
        Dictionary<int, int> commandDic = rowCfgStage.commandDic;
        List<WaitingCommandObj> originalObjCommandList = new List<WaitingCommandObj>();
        foreach (var pair in commandDic)
        {
            GameObject obj = CreatWaitingObj();
            obj.transform.SetParent(_view.waitingObjContainer);
            WaitingCommandObj waitingObjTemp=obj.GetComponent<WaitingCommandObj>();
            CommandType commandEnum = (CommandType) pair.Key;
            RowCfgCommand rowCfgCommand = ConfigManager.Instance.cfgCommand[commandEnum.ToString()];
            waitingObjTemp.transform.Find("ClickBtn").Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
            waitingObjTemp.Init(commandEnum, pair.Value,rowCfgCommand.needTime);
            
            UpdateWaitingObjView(waitingObjTemp);
            originalObjCommandList.Add(waitingObjTemp);
        }

        _model.OnInit(originalObjCommandList, rowCfgStage);
    }

    /// <summary>
    /// 新增一个指令输入
    /// </summary>
    /// <param name="commandEnum"></param>
    /// <param name="needTime"></param>
    /// <returns></returns>
    public UsedCommandObj AddNewUsedObj(CommandType commandEnum,int needTime)
    {
        GameObject obj = CreatUsedObj();
        obj.transform.SetParent(_view.usedObjContainer);
        UsedCommandObj usedObj = obj.GetComponent<UsedCommandObj>();
        usedObj.Init(commandEnum,1,needTime);
        return usedObj;
    }
    
    /// <summary>
    /// 更新单个可选指令的显示
    /// </summary>
    /// <param name="waitingObj"></param>
    public void UpdateWaitingObjView(WaitingCommandObj waitingObj)
    {
        SetWaitingObjClickBtnMaskActive(waitingObj,waitingObj.count>0);//当waitingObj的count为0时，关闭waitingObj，否则启用
        waitingObj.countText.text = "x" + waitingObj.count.ToString();
        waitingObj.needTimeText.text = waitingObj.needTime.ToString() + "s";
    }
    
    /// <summary>
    /// 更新最后一个输入的指令的显示
    /// </summary>
    /// <param name="usedObj"></param>
    public void UpdateLastUsedObjView(UsedCommandObj usedObj)
    {
        usedObj.countText.text = "x" +usedObj.count.ToString();
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
    /// 当waitingObj的count为0时，开启btn的遮罩阻挡点击
    /// </summary>
    /// <param name="waitingObj"></param>
    /// <param name="canClick"></param>
    private void SetWaitingObjClickBtnMaskActive(WaitingCommandObj waitingObj,bool canClick)
    {
        waitingObj.transform.Find("ClickBtnMask").gameObject.SetActive(!canClick);
    }

    /// <summary>
    /// 生成一个新的waitingObj
    /// </summary>
    /// <returns></returns>
    private GameObject CreatWaitingObj()
    {
        GameObject waitingObj =GameManager.Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["WaitingCommandObj"].prefabPath));
        waitingObj.transform.Find("ClickBtn").GetComponent<Button>().onClick.AddListener(()=>
        {
            ClickWaitingObj(waitingObj);
        });
        return waitingObj;
    }
    
    /// <summary>
    /// 生成一个新的usedObj
    /// </summary>
    /// <returns></returns>
    private GameObject CreatUsedObj()
    {
        GameObject usedObj=GameManager.Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["UsedCommandObj"].prefabPath));
        usedObj.transform.Find("ClickBtn").GetComponent<Button>().onClick.AddListener(()=>
        {
            ClickUsedObj(usedObj);
        });
        return usedObj;
    }

    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingObj"></param>
    private void ClickWaitingObj(GameObject waitingObj)
    {
        _model.ClickWaitingObj(waitingObj.GetComponent<WaitingCommandObj>());
    }

    /// <summary>
    /// 点击usedObj
    /// </summary>
    /// <param name="usedObj"></param>
    private void ClickUsedObj(GameObject usedObj)
    {
        _model.ClickUsedObj(usedObj.GetComponent<UsedCommandObj>());
    }
    
}
