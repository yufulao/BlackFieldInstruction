using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.UI;

public class CommandManager : MonoSingleton<CommandManager>
{
    public Transform usedObjContainer;
    public Transform waitingObjContainer;

    private CommandViewCtrl _viewCtrl;
    
    private CommandMainModel _commandModel;//动态的，会切换

    public void OpenCommandView()
    {
        StartCoroutine(UiManager.Instance.OpenWindow("CommandView", (ctrl) =>
        {
            _viewCtrl=ctrl as CommandViewCtrl;
            usedObjContainer=_viewCtrl.root.Find("DownFrame").Find("UsedCommandObjList").Find("Viewport").Find("UsedObjContainer");
            waitingObjContainer=_viewCtrl.root.Find("DownFrame").Find("WaitingCommandObjList").Find("Viewport").Find("WaitingObjContainer");
        }));
    }

    /// <summary>
    /// 重新获取场景中的commandModel和commandView
    /// </summary>
    /// <param name="rowCfgStage"></param>
    public void ReloadCommandModel(RowCfgStage rowCfgStage)
    {
        StartCoroutine(InitOriginalObjCommandList(rowCfgStage));
    }

    private IEnumerator InitOriginalObjCommandList(RowCfgStage rowCfgStage)
    {
        Dictionary<int, int> commandDic = rowCfgStage.commandDic;
        List<WaitingCommandObj> originalObjCommandList = new List<WaitingCommandObj>();
        foreach (var pair in commandDic)
        {
            yield return StartCoroutine(CreatWaitingObj((obj) =>
            {
                obj.transform.SetParent(waitingObjContainer);
                WaitingCommandObj waitingObjTemp=obj.GetComponent<WaitingCommandObj>();
                CommandEnum commandEnum = (CommandEnum) pair.Key;
                //Debug.Log("i:"+i+","+"commandDic[i]:"+commandDic[i]);
                RowCfgCommand rowCfgCommand = ConfigManager.Instance.cfgCommand[commandEnum.ToString()];
                //Debug.Log("commandEnum:"+commandEnum+","+"count:"+commandDic[i]+","+"needTime:"+rowCfgCommand.needTime);
                waitingObjTemp.Init(commandEnum, pair.Value,rowCfgCommand.needTime);
            
                UpdateWaitingObjView(waitingObjTemp);
                originalObjCommandList.Add(waitingObjTemp);
            }));
        }
        _commandModel=GameObject.FindObjectOfType<CommandMainModel>();
        _commandModel.InitModel(originalObjCommandList);
    }

    /// <summary>
    /// 点击waitingObj
    /// </summary>
    /// <param name="waitingObj"></param>
    public void ClickWaitingObj(GameObject waitingObj)
    {
        _commandModel.ClickWaitingObj(waitingObj.GetComponent<WaitingCommandObj>());
    }

    public void ClickUsedObj(GameObject usedObj)
    {
        _commandModel.ClickUsedObj(usedObj.GetComponent<UsedCommandObj>());
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

    public IEnumerator AddNewUsedObj(CommandEnum commandEnum,int needTime,Action<UsedCommandObj> callback)
    {
        yield return CreatUsedObj((obj) =>
        {
            obj.transform.SetParent(usedObjContainer);
            UsedCommandObj usedObj = obj.GetComponent<UsedCommandObj>();
            usedObj.Init(commandEnum,1,needTime);
            callback?.Invoke(usedObj);
        });
    }
    
    public void NoWaitingObj()
    {
        
    }

    public void NoUsedObj()
    {
        
    }
    
    public IEnumerator CreatWaitingObj(Action<GameObject> callback)
    {
        yield return AssetManager.Instance.LoadAssetAsync<GameObject>(
            ConfigManager.Instance.cfgPrefab["WaitingCommandObj"].prefabPath, (obj) =>
            {
                GameObject waitingObj =Instantiate(obj);
                waitingObj.transform.Find("ClickBtn").GetComponent<Button>().onClick.AddListener(()=>
                {
                    CommandManager.Instance.ClickWaitingObj(waitingObj);
                });
                callback?.Invoke(waitingObj);
            });
    }
    
    public IEnumerator CreatUsedObj(Action<GameObject> callback)
    {
        yield return AssetManager.Instance.LoadAssetAsync<GameObject>(
            ConfigManager.Instance.cfgPrefab["UsedCommandObj"].prefabPath, (obj) =>
            {
                GameObject usedObj =Instantiate(obj);
                usedObj.transform.Find("ClickBtn").GetComponent<Button>().onClick.AddListener(()=>
                {
                    CommandManager.Instance.ClickUsedObj(usedObj);
                });
                callback?.Invoke(usedObj);
            });
    }
}
