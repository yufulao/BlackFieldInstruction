using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UsedCommandItem : CommandItem
{
    [SerializeField] private Text currentTimeText;

    [HideInInspector] public int currentTime;

    private Action<PointerEventData> _scrollOnBeginDrag;
    private Action<PointerEventData> _scrollOnDrag;
    private Action<PointerEventData> _scrollOnEndDrag;


    /// <summary>
    /// 生成usedObj时初始化相关属性
    /// </summary>
    /// <param name="commandEnumT"></param>
    /// <param name="countT"></param>
    /// <param name="currentTimeT"></param>
    public void Init(CommandType commandEnumT, int countT, int needTimeT, int currentTimeT,Transform onDragParent
        , Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    {
        base.Init(commandEnumT, countT, needTimeT);
        currentTime = currentTimeT;
        _scrollOnBeginDrag = scrollOnBeginDrag;
        _scrollOnDrag = scrollOnDrag;
        _scrollOnEndDrag = scrollOnEndDrag;

        clickBtn.transform.Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
        clickBtn.transform.GetComponent<Button>().onClick.AddListener(() => { CommandUICtrl.Instance.ClickUsedItem(this); });
        clickBtn.transform.GetComponent<UIDragComponent>().InitDragComponent(onDragParent, DragFilter, () => UsedBtnOnBeginDrag(), OnCommandItemEndDragCallback
            , _scrollOnBeginDrag, _scrollOnDrag, _scrollOnEndDrag);

        UpdateLastUsedObjView();
    }

    /// <summary>
    /// 更新最后一个输入的指令的显示
    /// </summary>
    public void UpdateLastUsedObjView()
    {
        countText.text = "x" + count.ToString();
        currentTimeText.text = currentTime.ToString() + "s";
    }

    
    
    
    

    private bool DragFilter(GameObject obj)
    {
        //Debug.Log(obj.name);
        if (obj.name == "WaitingCommandItemList")
        {
            return true; //保留
        }

        return false; //移除
    }

    private void OnCommandItemEndDragCallback(List<GameObject> resultObjs)
    {
        if (resultObjs == null)
        {
            UsedBtnOnEndDragFail();
            return;
        }

        for (int i = 0; i < resultObjs.Count; i++)
        {
            if (resultObjs[i].name == "WaitingCommandItemList")
            {
                CommandUICtrl.Instance.ClickUsedItem(this);
                return;
            }
        }

        UsedBtnOnEndDragFail();
    }

    /// <summary>
    /// 只剩下一个usedBtn拖拽失败的事件，恢复count和currentTime显示
    /// </summary>
    private void UsedBtnOnEndDragFail()
    {
        if (count <= 1)
        {
            canvasGroup.alpha = 1;
        }
    }

    /// <summary>
    /// 只剩下一个usedBtn开始拖拽的事件，隐藏count和currentTime显示
    /// </summary>
    private void UsedBtnOnBeginDrag()
    {
        if (count <= 1)
        {
            canvasGroup.alpha = 0;
        }
    }
}