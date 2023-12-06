using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandItem : MonoBehaviour
{
    [SerializeField] protected Text countText;
    [SerializeField] protected Text timeText;
    [SerializeField] private Button clickBtn;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] private UIDragComponent btnDragCmp;
    [HideInInspector]public CommandItemInfo itemInfo;


    public virtual void Init(CommandType commandEnum, Transform onDragParent,CommandItemInfo itemInfoT)
    {
        transform.Find("Decorate").Find("ClickBtnBg").Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
        clickBtn.transform.Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
        btnDragCmp.InitDragComponent(onDragParent);
        itemInfo = itemInfoT;
    }

    /// <summary>
    /// 更新单个可选指令的显示
    /// </summary>
    public virtual void Refresh(int count, int time)
    {
        countText.text = "x" + count.ToString();
        timeText.text = time.ToString() + "s";
    }

    /// <summary>
    /// 注册btn点击事件
    /// </summary>
    /// <param name="btnOnClick"></param>
    public void SetBtnOnClick(Action<CommandItem> btnOnClick)
    {
        clickBtn.transform.GetComponent<Button>().onClick.AddListener(() => { btnOnClick?.Invoke(this); });
    }

    /// <summary>
    /// 设置拖拽事件
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="onBeginDrag"></param>
    /// <param name="onEndDrag"></param>
    public void SetDragAction(Func<GameObject, bool> filter, Action<CommandItem> onBeginDrag, Action<CommandItem, List<GameObject>> onEndDrag)
    {
        btnDragCmp.SetDragAction(filter, () => { onBeginDrag?.Invoke(this); }, (objs) => { onEndDrag?.Invoke(this, objs); });
    }

    /// <summary>
    /// 设置无效拖拽事件，即水平拖拽事件
    /// </summary>
    /// <param name="validOnBeginDrag"></param>
    /// <param name="validOnDrag"></param>
    /// <param name="validOnEndDrag"></param>
    public void SetValidDragAction(Action<PointerEventData> validOnBeginDrag, Action<PointerEventData> validOnDrag, Action<PointerEventData> validOnEndDrag)
    {
        btnDragCmp.SetValidDragAction(validOnBeginDrag, validOnDrag, validOnEndDrag);
    }

    /// <summary>
    /// 显示item
    /// </summary>
    public void ShowItem()
    {
        canvasGroup.alpha = 1;
    }

    /// <summary>
    /// 隐藏item
    /// </summary>
    public void HideItem()
    {
        canvasGroup.alpha = 0;
    }
}