using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UsedCommandObj : CommandObj
{
    [HideInInspector] public int currentTime;

    public Text currentTimeText;


    /// <summary>
    /// 生成usedObj时初始化相关属性
    /// </summary>
    /// <param name="commandEnumT"></param>
    /// <param name="countT"></param>
    /// <param name="currentTimeT"></param>
    public void Init(CommandType commandEnumT, int countT, int currentTimeT)
    {
        base.Init(commandEnumT, countT);
        currentTime = currentTimeT;
    }

    public bool DragFilter(GameObject obj)
    {
        //Debug.Log(obj.name);
        if (obj.name == "WaitingCommandObjList")
        {
            return true; //保留
        }

        return false; //移除
    }

    public void OnCommandObjEndDragCallback(List<GameObject> resultObjs)
    {
        if (resultObjs == null)
        {
            CommandViewCtrl.Instance.UsedBtnOnEndDragFail(this);
            return;
        }

        for (int i = 0; i < resultObjs.Count; i++)
        {
            if (resultObjs[i].name == "WaitingCommandObjList")
            {
                CommandViewCtrl.Instance.ClickUsedObj(this);
                return;
            }
        }

        CommandViewCtrl.Instance.UsedBtnOnEndDragFail(this);
    }
    

}