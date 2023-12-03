using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandViewModel : MonoBehaviour
{
    private RowCfgStage _rowCfgStage;
    private List<UsedCommandObj> _usedObjList;
    private List<WaitingCommandObj> _waitingObjList;

    private int _currentNeedTime;


    /// <summary>
    /// 初始化本关卡所有可选指令
    /// </summary>
    /// <param name="originalObjCommandList">初始指令列表</param>
    /// <returns></returns>
    public void OnInit(List<WaitingCommandObj> originalObjCommandList, RowCfgStage rowCfgStage)
    {
        _rowCfgStage = rowCfgStage;
        _usedObjList = new List<UsedCommandObj>();
        _waitingObjList = new List<WaitingCommandObj>();
        _currentNeedTime = 0;
        _waitingObjList = originalObjCommandList;
    }

    /// <summary>
    /// 更新当前所需总时间
    /// </summary>
    /// <param name="addTime"></param>
    public void UpdateCurrentNeedTime(int addTime)
    {
        _currentNeedTime += addTime;
        CommandViewCtrl.Instance.UpdateCurrentTimeText(_currentNeedTime);
    }

    /// <summary>
    /// 获取usedCommandList
    /// </summary>
    /// <returns></returns>
    public List<UsedCommandObj> GetUsedObjList()
    {
        return _usedObjList;
    }

    /// <summary>
    /// usedObj加一
    /// </summary>
    /// <param name="usedObjContainer"></param>
    public void AddUsedCommand(WaitingCommandObj waitingObj, Transform usedObjContainer)
    {
        UsedCommandObj usedObj = null;
        if (_usedObjList.Count == 0 || _usedObjList[_usedObjList.Count - 1].commandEnum != waitingObj.commandEnum)
        {
            usedObj =
                Instantiate(
                    AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["UsedCommandObj"]
                        .prefabPath), usedObjContainer).GetComponent<UsedCommandObj>();
            usedObj.Init(waitingObj.commandEnum, 1, waitingObj.needTime);
            CommandViewCtrl.Instance.CreatUsedBtn(usedObj);
            _usedObjList.Add(usedObj);
            UpdateUsedObj(usedObj);
            return;
        }

        //最新的usedObj是同类command
        usedObj = _usedObjList[_usedObjList.Count - 1];
        usedObj.count++;
        if (usedObj.btnList.Count > usedObj.count)
        {
            usedObj.btnList[usedObj.count].SetActive(true);
        }
        else
        {
            CommandViewCtrl.Instance.CreatUsedBtn(usedObj);
        }

        UpdateUsedObj(usedObj);
    }

    /// <summary>
    /// waitingObj加1
    /// </summary>
    /// <param name="commandEnum"></param>
    public WaitingCommandObj AddWaitingCommand(CommandType commandEnum)
    {
        WaitingCommandObj waitingObj = null;
        for (int i = 0; i < _waitingObjList.Count; i++)
        {
            if (_waitingObjList[i].commandEnum == commandEnum)
            {
                waitingObj = _waitingObjList[i];
                break;
            }
        }

        waitingObj.count++;
        waitingObj.btnList[waitingObj.count - 1].SetActive(true);

        CommandViewCtrl.Instance.UpdateWaitingObjView(waitingObj);
        return waitingObj;
    }

    /// <summary>
    /// waitingObj减一
    /// </summary>
    /// <param name="waitingObj"></param>
    public void RemoveWaitingCommand(WaitingCommandObj waitingObj)
    {
        waitingObj.count--;
        waitingObj.btnList[waitingObj.count].gameObject.SetActive(false);
        if (_waitingObjList.Count <= 0)
        {
            CommandViewCtrl.Instance.NoWaitingObj();
        }

        CommandViewCtrl.Instance.UpdateWaitingObjView(waitingObj);
    }

    /// <summary>
    /// usedObj减一
    /// </summary>
    /// <param name="usedObj"></param>
    public void RemoveUsedCommand(UsedCommandObj usedObj)
    {
        usedObj.count--;
        usedObj.btnList[usedObj.count].gameObject.SetActive(false);

        if (usedObj.count <= 0)
        {
            int removeUsedObjIndex = _usedObjList.IndexOf(usedObj);
            _usedObjList.RemoveAt(removeUsedObjIndex);
            Destroy(usedObj.gameObject); //对象池处理==================================
            CheckSameAfterRemoveUsedObj(removeUsedObjIndex);
        }

        if (_usedObjList.Count <= 0)
        {
            CommandViewCtrl.Instance.NoUsedObj();
        }
        UpdateUsedObj(usedObj);
    }

    public void CheckSameAfterRemoveUsedObj(int removeUsedObjIndex)
    {
        if (_usedObjList.Count < 2 || removeUsedObjIndex <= 0 || removeUsedObjIndex >= _usedObjList.Count)
        {
            return;
        }

        if (_usedObjList[removeUsedObjIndex - 1].commandEnum == _usedObjList[removeUsedObjIndex].commandEnum)
        {
            var lastUsedObj = _usedObjList[removeUsedObjIndex - 1];
            var nextUsedObj = _usedObjList[removeUsedObjIndex];
            for (int i = 0; i < nextUsedObj.btnList.Count; i++)
            {
                nextUsedObj.btnList[i].transform.SetParent(lastUsedObj.clickBtnContainer);
                nextUsedObj.btnList[i].transform.position=Vector3.zero;
                nextUsedObj.btnList[i].GetComponent<Button>().onClick.RemoveAllListeners();
                nextUsedObj.btnList[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    CommandViewCtrl.Instance.ClickUsedObj(lastUsedObj);
                });
                lastUsedObj.btnList.Add(nextUsedObj.btnList[i]);
            }
            
            lastUsedObj.count += nextUsedObj.count;
            _usedObjList.Remove(nextUsedObj);
            Destroy(nextUsedObj.gameObject); //对象池处理==================================
            UpdateUsedObj(lastUsedObj);
        }

    }

    /// <summary>
    /// 更新最新的usedObj的当前时间
    /// </summary>
    private void UpdateUsedObj(UsedCommandObj usedCommandObj)
    {
        if (_usedObjList.Count == 0)
        {
            return;
        }

        UsedCommandObj lastUsedObj = _usedObjList[_usedObjList.Count - 1];
        lastUsedObj.currentTime = _currentNeedTime;
        CommandViewCtrl.Instance.UpdateLastUsedObjView(lastUsedObj);
        CommandViewCtrl.Instance.UpdateLastUsedObjView(usedCommandObj);
    }
}