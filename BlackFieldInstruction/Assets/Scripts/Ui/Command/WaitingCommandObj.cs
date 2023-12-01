using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingCommandObj : MonoBehaviour
{
    [HideInInspector]
    public CommandType commandEnum;
    [HideInInspector]
    public int needTime;
    [HideInInspector]
    public int count;

    public Text countText;
    public Text needTimeText;
    
    /// <summary>
    /// 生成waitingObj时初始化相关属性
    /// </summary>
    /// <param name="commandEnumT"></param>
    /// <param name="countT"></param>
    /// <param name="needTimeT"></param>
    public void Init(CommandType commandEnumT,int countT,int needTimeT)
    {
        commandEnum = commandEnumT;
        count = countT;
        needTime = needTimeT;
    }
}
