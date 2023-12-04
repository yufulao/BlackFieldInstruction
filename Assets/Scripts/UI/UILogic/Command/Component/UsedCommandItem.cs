using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UsedCommandItem : CommandItem
{
    [HideInInspector] public int currentTime;


    public void Init(CommandType commandEnumT, int countT, int needTimeT, int currentTimeT, Transform onDragParent, UnityAction btnClickCallbackT
        , Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    {
        base.Init(commandEnumT, countT, needTimeT, btnClickCallbackT,scrollOnBeginDrag,scrollOnDrag,scrollOnEndDrag);
        currentTime = currentTimeT;

        clickBtn.transform.GetComponent<UIDragComponent>().InitDragComponent(onDragParent, DragFilter, () => UsedBtnOnBeginDrag(), OnCommandItemEndDragCallback
            , _scrollOnBeginDrag, _scrollOnDrag, _scrollOnEndDrag);

        RefreshView();
    }

    /// <summary>
    /// 更新最后一个输入的指令的显示
    /// </summary>
    public void RefreshView()
    {
        countText.text = "x" + count.ToString();
        timeText.text = currentTime.ToString() + "s";
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
                btnClickCallback.Invoke();
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