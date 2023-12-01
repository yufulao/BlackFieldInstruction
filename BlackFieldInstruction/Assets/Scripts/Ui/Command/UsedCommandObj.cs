using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsedCommandObj : MonoBehaviour
{
    [HideInInspector]
    public CommandType commandEnum;
    [HideInInspector]
    public int count;
    [HideInInspector]
    public int currentTime;
    
    public Text currentTimeText;
    public Text countText;

    /// <summary>
    /// 生成usedObj时初始化相关属性
    /// </summary>
    /// <param name="commandEnumT"></param>
    /// <param name="countT"></param>
    /// <param name="currentTimeT"></param>
    public void Init(CommandType commandEnumT,int countT,int currentTimeT)
    {
        commandEnum = commandEnumT;
        count = countT;
        currentTime = currentTimeT;
        transform.Find("ClickBtn").Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
    }
}
