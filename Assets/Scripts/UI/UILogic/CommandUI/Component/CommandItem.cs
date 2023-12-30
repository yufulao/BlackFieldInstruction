using System;
using System.Collections.Generic;
using Rabi;
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
    [SerializeField] private GameObject clickBtnMask;

    [SerializeField] private UIDragComponent btnDragCmp;
    // [HideInInspector]public CommandItemInfo itemInfo;


    public virtual void Init(CommandType commandEnum, Transform onDragParent /*,CommandItemInfo itemInfoT*/)
    {
        Sprite commandTypeSprite = AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgSprite[commandEnum.ToString()].spritePath);
        transform.Find("Decorate").Find("ClickBtnBg").Find("CommandTypeSprite").GetComponent<Image>().sprite = commandTypeSprite;
        clickBtn.transform.Find("CommandTypeSprite").GetComponent<Image>().sprite = commandTypeSprite;
        btnDragCmp.InitDragComponent(onDragParent);
        // itemInfo = itemInfoT;
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
        btnDragCmp.SetDragAction(filter, (eventData) =>
        {
            onBeginDrag?.Invoke(this);
            DeselectClickBtnOnBeginDrag();
        }, (objs,isValidDrag) =>
        {
            onEndDrag?.Invoke(this, objs);
            EnableClickBtnOnEndDrag(isValidDrag);
        });
    }

    /// <summary>
    /// 设置无效拖拽事件，即水平拖拽事件
    /// </summary>
    /// <param name="invalidOnBeginDrag"></param>
    /// <param name="invalidOnDrag"></param>
    /// <param name="invalidOnEndDrag"></param>
    public void SetInvalidDragAction(Action<PointerEventData> invalidOnBeginDrag, Action<PointerEventData> invalidOnDrag, Action<PointerEventData> invalidOnEndDrag)
    {
        btnDragCmp.SetInvalidDragAction(invalidOnBeginDrag, invalidOnDrag, (eventData)=>
        {
            invalidOnEndDrag?.Invoke(eventData);
            EnableClickBtn();
        });
    }

    /// <summary>
    /// 控制btn遮挡的开关
    /// </summary>
    /// <param name="active"></param>
    public void UpdateBtnMask( bool active)
    {
        clickBtnMask.SetActive(active);
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
    
    /// <summary>
    /// 判断为拖拽的时候，取消btn的可交互
    /// </summary>
    private void DeselectClickBtnOnBeginDrag()
    {
        clickBtn.interactable = false;
    }

    /// <summary>
    /// 拖拽结束时恢复btn的可交互
    /// </summary>
    /// <param name="isValidDrag"></param>
    private void EnableClickBtnOnEndDrag(bool isValidDrag)
    {
        // Debug.Log(isValidDrag);
        if (isValidDrag)
        {
            EnableClickBtn();
        }
    }
    
    /// <summary>
    /// 恢复btn的可交互
    /// </summary>
    private void EnableClickBtn()
    {
        clickBtn.interactable = true;
    }
}