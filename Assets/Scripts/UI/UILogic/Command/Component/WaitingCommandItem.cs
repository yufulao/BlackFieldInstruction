using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaitingCommandItem : CommandItem
{
    [SerializeField] private Text needTimeText;
    [SerializeField] private GameObject clickBtnMask;

    private Action<PointerEventData> _scrollOnBeginDrag;
    private Action<PointerEventData> _scrollOnDrag;
    private Action<PointerEventData> _scrollOnEndDrag;


    public void Init(CommandType commandEnumT, int countT, int needTimeT,Transform onDragParent
        , Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    {
        base.Init(commandEnumT, countT, needTimeT);
        _scrollOnBeginDrag = scrollOnBeginDrag;
        _scrollOnDrag = scrollOnDrag;
        _scrollOnEndDrag = scrollOnEndDrag;

        clickBtn.transform.Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
        clickBtn.transform.GetComponent<Button>().onClick.AddListener(() => { CommandUICtrl.Instance.ClickWaitingItem(this); });
        clickBtn.transform.GetComponent<UIDragComponent>().InitDragComponent(onDragParent, DragFilter, null, OnCommandItemEndDragCallback
            , _scrollOnBeginDrag, _scrollOnDrag, _scrollOnEndDrag);

        UpdateView();
    }

    /// <summary>
    /// 更新单个可选指令的显示
    /// </summary>
    public void UpdateView()
    {
        clickBtnMask.SetActive(count <= 0); //当waitingObj的count为0时，关闭waitingObj，否则启用
        countText.text = "x" + count.ToString();
        needTimeText.text = needTime.ToString() + "s";
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
                CommandUICtrl.Instance.ClickWaitingItem(this);
                break;
            }
        }
    }
}