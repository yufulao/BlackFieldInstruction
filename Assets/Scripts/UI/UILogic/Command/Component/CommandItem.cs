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

    [HideInInspector]public CommandType cacheCommandEnum;
    [HideInInspector]public int cacheCount;
    [HideInInspector]public int cacheTime;
    protected Action<CommandItem> cacheBtnOnClick;


    public virtual void Init(CommandType commandEnum, Transform onDragParent,int count,int time)
    {
        cacheCommandEnum = commandEnum;
        cacheCount = count;
        cacheTime = time;
        transform.Find("Decorate").Find("ClickBtnBg").Find("Text (Legacy)").GetComponent<Text>().text = cacheCommandEnum.ToString();
        clickBtn.transform.Find("Text (Legacy)").GetComponent<Text>().text = cacheCommandEnum.ToString();
        btnDragCmp.InitDragComponent(onDragParent);
    }

    public virtual void SetAction(Action<CommandItem> btnOnClick, Func<GameObject, bool> filter, Action onBeginDrag, Action<List<GameObject>> onEndDrag, Action<PointerEventData> scrollOnBeginDrag,
        Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    {
        cacheBtnOnClick = btnOnClick;
        clickBtn.transform.GetComponent<Button>().onClick.AddListener(() => { btnOnClick?.Invoke(this); });
        btnDragCmp.SetDragAction(filter, onBeginDrag, onEndDrag);
        btnDragCmp.SetValidDragAction(scrollOnBeginDrag, scrollOnDrag, scrollOnEndDrag);
    }

    /// <summary>
    /// 更新单个可选指令的显示
    /// </summary>
    public virtual void Refresh()
    {
        countText.text = "x" + cacheCount.ToString();
        timeText.text = cacheTime.ToString() + "s";
    }

}