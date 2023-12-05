using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UsedCommandItem : CommandItem
{
    [HideInInspector] public int currentTime;


    // public void Init(CommandType commandEnumT, int cacheCountT, int cacheNeedTimeT, int currentTimeT, Transform onDragParent, UnityAction btnClickCallbackT
    //     , Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    // {
    //     base.Init(commandEnumT, cacheCountT, cacheNeedTimeT, btnClickCallbackT,scrollOnBeginDrag,scrollOnDrag,scrollOnEndDrag);
    //     currentTime = currentTimeT;
    //
    //     clickBtn.transform.GetComponent<UIDragComponent>().InitDragComponent(onDragParent, DragFilter, () => UsedBtnOnBeginDrag(), OnCommandItemEndDragCallback
    //         , _scrollOnBeginDrag, _scrollOnDrag, _scrollOnEndDrag);
    //
    //     RefreshView();
    // }

    public void SetAction(Action<CommandItem> btnOnClick, Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    {
        base.SetAction(btnOnClick, DragFilter, UsedBtnOnBeginDrag, OnCommandItemEndDragCallback, scrollOnBeginDrag, scrollOnDrag, scrollOnEndDrag);
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
                cacheBtnOnClick.Invoke(this);
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
        if (cacheCount <= 1)
        {
            canvasGroup.alpha = 1;
        }
    }

    /// <summary>
    /// 只剩下一个usedBtn开始拖拽的事件，隐藏count和currentTime显示
    /// </summary>
    private void UsedBtnOnBeginDrag()
    {
        if (cacheCount <= 1)
        {
            canvasGroup.alpha = 0;
        }
    }
}