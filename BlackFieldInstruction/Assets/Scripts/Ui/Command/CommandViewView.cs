using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandViewView
{
    public Transform root;
    public Transform usedObjContainer;
    public Transform waitingObjContainer;
    public Button startBtn;

    public void OnInit(Transform viewRoot)
    {
        root = viewRoot;
        usedObjContainer=root.Find("DownFrame").Find("UsedCommandObjList").Find("Viewport").Find("UsedObjContainer");
        waitingObjContainer=root.Find("DownFrame").Find("WaitingCommandObjList").Find("Viewport").Find("WaitingObjContainer");
        startBtn = root.Find("UpFrame").Find("StartBtn").GetComponent<Button>();
    }
}
