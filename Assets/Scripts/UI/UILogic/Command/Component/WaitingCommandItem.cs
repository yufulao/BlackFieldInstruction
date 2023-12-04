using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaitingCommandItem : CommandItem
{
    [SerializeField] private GameObject clickBtnMask;


    public void Init(CommandType commandEnumT, int countT, int needTimeT, Transform onDragParent, UnityAction btnClickCallbackT
        , Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    {
        base.Init(commandEnumT, countT, needTimeT, btnClickCallbackT, scrollOnBeginDrag,scrollOnDrag,scrollOnEndDrag);

        clickBtn.transform.GetComponent<UIDragComponent>().InitDragComponent(onDragParent, DragFilter, null, OnCommandItemEndDragCallback
            , _scrollOnBeginDrag, _scrollOnDrag, _scrollOnEndDrag);

        RefreshView();
    }

    /// <summary>
    /// 更新单个可选指令的显示
    /// </summary>
    public void RefreshView()
    {
        clickBtnMask.SetActive(count <= 0); //当waitingObj的count为0时，关闭waitingObj，否则启用
        countText.text = "x" + count.ToString();
        timeText.text = needTime.ToString() + "s";
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
                btnClickCallback.Invoke();
                break;
            }
        }
    }
}