using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaitingCommandObj : CommandObj
{
    [HideInInspector]
    public int needTime;
    
    public Text needTimeText;
    

    /// <summary>
    /// 生成waitingObj时初始化相关属性
    /// </summary>
    /// <param name="commandEnumT"></param>
    /// <param name="countT"></param>
    /// <param name="needTimeT"></param>
    public void Init(CommandType commandEnumT,int countT,int needTimeT)
    {
        base.Init(commandEnumT, countT);
        needTime = needTimeT;
        transform.Find("ClickBtnBg").Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
    }
    
    public bool DragFilter(GameObject obj)
    {
        if (obj.name == "UsedCommandObjList")
        {
            return true; //保留
        }

        return false;//移除
    }
    
    public void OnCommandObjEndDragCallback(List<GameObject> resultObjs)
    {
        if (resultObjs == null)
        {
            return;
        }

        for (int i = 0; i < resultObjs.Count; i++)
        {
            if (resultObjs[i].name=="UsedCommandObjList")
            {
                CommandViewCtrl.Instance.ClickWaitingObj(this);
                break;
            }
        }
    }
    

}
