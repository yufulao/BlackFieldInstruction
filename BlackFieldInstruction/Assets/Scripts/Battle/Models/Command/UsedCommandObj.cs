using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsedCommandObj : MonoBehaviour
{
    [HideInInspector]
    public CommandEnum commandEnum;
    [HideInInspector]
    public int count;
    [HideInInspector]
    public int currentTime;
    
    public Text currentTimeText;
    public Text countText;

    public void Init(CommandEnum commandEnumT,int countT,int currentTimeT)
    {
        commandEnum = commandEnumT;
        count = countT;
        currentTime = currentTimeT;
    }
}
