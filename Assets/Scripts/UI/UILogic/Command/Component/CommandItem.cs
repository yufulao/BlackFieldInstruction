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
    protected Action<CommandItem> cacheBtnOnClick;


    public virtual void Init(CommandType commandEnum, Transform onDragParent)
    {
        transform.Find("Decorate").Find("ClickBtnBg").Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
        clickBtn.transform.Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
        btnDragCmp.InitDragComponent(onDragParent);
    }

    /// <summary>
    /// 更新单个可选指令的显示
    /// </summary>
    public virtual void Refresh(int count, int time)
    {
        countText.text = "x" + count.ToString();
        timeText.text = time.ToString() + "s";
    }

    public void SetBtnOnClick(Action<CommandItem> btnOnClick)
    {
        clickBtn.transform.GetComponent<Button>().onClick.AddListener(() => { btnOnClick?.Invoke(this); });
    }

    public void SetDragAction(Func<GameObject, bool> filter, Action<CommandItem> onBeginDrag, Action<CommandItem, List<GameObject>> onEndDrag)
    {
        btnDragCmp.SetDragAction(filter, () => { onBeginDrag?.Invoke(this); }, (objs) => { onEndDrag?.Invoke(this, objs); });
    }

    public void SetValidDragAction(Action<PointerEventData> validOnBeginDrag, Action<PointerEventData> validOnDrag, Action<PointerEventData> validOnEndDrag)
    {
        btnDragCmp.SetValidDragAction(validOnBeginDrag, validOnDrag, validOnEndDrag);
    }


    public void ShowItem()
    {
        canvasGroup.alpha = 1;
    }

    public void HideItem()
    {
        canvasGroup.alpha = 0;
    }
}