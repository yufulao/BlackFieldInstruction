using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaitingCommandItem : CommandItem
{
    [SerializeField] private GameObject clickBtnMask;


    // public void Init(CommandType commandEnumT, int cacheCountT, int cacheNeedTimeT, Transform onDragParent, UnityAction btnClickCallbackT
    //     , Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    // {
    //     base.Init(commandEnumT, cacheCountT, cacheNeedTimeT, btnClickCallbackT, scrollOnBeginDrag,scrollOnDrag,scrollOnEndDrag);
    //
    //     clickBtn.transform.GetComponent<UIDragComponent>().InitDragComponent(onDragParent, DragFilter, null, OnCommandItemEndDragCallback
    //         , _scrollOnBeginDrag, _scrollOnDrag, _scrollOnEndDrag);
    //
    //     RefreshView();
    // }
    
    public void SetAction(Action<CommandItem> btnOnClick, Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    {
        base.SetAction(btnOnClick, DragFilter, null, OnCommandItemEndDragCallback, scrollOnBeginDrag, scrollOnDrag, scrollOnEndDrag);
    }
    
    public override void Refresh()
    {
        base.Refresh();
        clickBtnMask.SetActive(cacheCount <= 0); //当waitingObj的cacheCount为0时，关闭waitingObj，否则启用
    }


    private bool DragFilter(GameObject obj)
    {
        if (obj.name == "UsedCommandItemList")
        {
            return true; //保留
        }

        return false; //移除
    }

    private void OnCommandItemEndDragCallback(List<GameObject> resultObjs)
    {
        if (resultObjs == null)
        {
            return;
        }

        for (int i = 0; i < resultObjs.Count; i++)
        {
            if (resultObjs[i].name == "UsedCommandItemList")
            {
                cacheBtnOnClick.Invoke(this);
                break;
            }
        }
    }
}