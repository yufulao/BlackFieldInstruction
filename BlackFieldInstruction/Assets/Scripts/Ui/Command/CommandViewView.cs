using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandViewView
{
    public Transform root;
    public Transform usedObjContainer;
    public Transform waitingObjContainer;
    public Button startBtn;
    public Text currentTimeText;
    public Text stageTimeText;
    public ScrollRect waitingCommandScroll;
    public ScrollRect usedCommandScroll;

    public void OnInit(Transform viewRoot)
    {
        root = viewRoot;
        usedObjContainer=root.Find("DownFrame").Find("UsedCommandObjList").Find("Viewport").Find("UsedObjContainer");
        waitingObjContainer=root.Find("DownFrame").Find("WaitingCommandObjList").Find("Viewport").Find("WaitingObjContainer");
        startBtn = root.Find("UpFrame").Find("StartBtn").GetComponent<Button>();
        currentTimeText=root.Find("UpFrame").Find("TimeShow").Find("CurrentTimeText").GetComponent<Text>();
        stageTimeText=root.Find("UpFrame").Find("TimeShow").Find("StageTimeText").GetComponent<Text>();
        waitingCommandScroll = root.Find("DownFrame").Find("WaitingCommandObjList").GetComponent<ScrollRect>();
        usedCommandScroll=root.Find("DownFrame").Find("UsedCommandObjList").GetComponent<ScrollRect>();
    }
    
    
}
