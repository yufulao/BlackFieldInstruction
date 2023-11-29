using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class CommandManager : MonoSingleton<CommandManager>
{
    public Transform usedObjContainer;
    public Transform waitingObjContainer;
    public GameObject usedObjPrefab;
    public GameObject waitingObjPrefab;
    
    private CommandMainModel _commandModel;//动态的，会切换

    public void LoadCommandUi(Transform commandUiRoot)
    {
        usedObjContainer=commandUiRoot.Find("DownFrame").Find("UsedCommandObjList").Find("Viewport").Find("UsedObjContainer");
        waitingObjContainer=commandUiRoot.Find("DownFrame").Find("WaitingCommandObjList").Find("Viewport").Find("WaitingObjContainer");
    }
    
    /// <summary>
    /// 重新获取场景中的commandModel和commandView
    /// </summary>
    /// <param name="rowCfgStage"></param>
    public void ReloadCommandModel(RowCfgStage rowCfgStage)
    {
        Dictionary<int, int> commandDic = rowCfgStage.commandDic;
        List<WaitingCommandObj> originalObjCommandList = new List<WaitingCommandObj>();
        foreach (var pair in commandDic)
        {
            WaitingCommandObj waitingObjTemp = Instantiate(waitingObjPrefab,waitingObjContainer).GetComponent<WaitingCommandObj>();
            CommandEnum commandEnum = (CommandEnum) pair.Key;
            //Debug.Log("i:"+i+","+"commandDic[i]:"+commandDic[i]);
            RowCfgCommand rowCfgCommand = ConfigManager.Instance.cfgCommand[commandEnum.ToString()];
            //Debug.Log("commandEnum:"+commandEnum+","+"count:"+commandDic[i]+","+"needTime:"+rowCfgCommand.needTime);
            waitingObjTemp.Init(commandEnum, pair.Value,rowCfgCommand.needTime);
            
            UpdateWaitingObjView(waitingObjTemp);
            originalObjCommandList.Add(waitingObjTemp);
        }

        _commandModel=GameObject.FindObjectOfType<CommandMainModel>();
        _commandModel.InitModel(originalObjCommandList);
    }
    
    
    
    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingObj"></param>
    public void ClickWaitingObj(WaitingCommandObj waitingObj)
    {
        _commandModel.ClickWaitingObj(waitingObj);
    }

    public void ClickUsedObj(UsedCommandObj usedObj)
    {
        _commandModel.ClickUsedObj(usedObj);
    }

    public void UpdateWaitingObjView(WaitingCommandObj waitingObj)
    {
        CloseAndOpenWaitingObj(waitingObj,waitingObj.count>0);//当waitingObj的count为0时，关闭waitingObj，否则启用
        waitingObj.countText.text = "x" + waitingObj.count.ToString();
        waitingObj.needTimeText.text = waitingObj.needTime.ToString() + "s";
    }

    public void UpdateLastUsedObjView(UsedCommandObj usedObj)
    {
        usedObj.countText.text = "x" +usedObj.count.ToString();
        usedObj.currentTimeText.text = usedObj.currentTime.ToString() + "s";
    }

    public void CloseAndOpenWaitingObj(WaitingCommandObj waitingObj,bool open)
    {
        waitingObj.gameObject.SetActive(open);
    }

    public UsedCommandObj AddNewUsedObj(CommandEnum commandEnum,int needTime)
    {
        UsedCommandObj usedObj = Instantiate(usedObjPrefab,usedObjContainer).GetComponent<UsedCommandObj>();//对象池处理，好像对象池也得开个新类，不然对象好像重复引用了============================================================
        usedObj.Init(commandEnum,1,needTime);
        return usedObj;
    }
    
    public void NoWaitingObj()
    {
        
    }

    public void NoUsedObj()
    {
        
    }
}
