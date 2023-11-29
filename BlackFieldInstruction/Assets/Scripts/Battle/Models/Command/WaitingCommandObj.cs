using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingCommandObj : MonoBehaviour
{
    [HideInInspector]
    public CommandEnum commandEnum;
    [HideInInspector]
    public int needTime;
    [HideInInspector]
    public int count;
    
    public Text countText;
    public Text needTimeText;
    
    public void Init(CommandEnum commandEnumT,int countT,int needTimeT)
    {
        commandEnum = commandEnumT;
        count = countT;
        needTime = needTimeT;
    }
}
