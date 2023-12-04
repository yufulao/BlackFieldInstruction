using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandItem : MonoBehaviour
{
    [SerializeField]protected Text countText;
    [SerializeField]protected Text timeText;
    [SerializeField]protected CanvasGroup canvasGroup;
    [SerializeField]protected Button clickBtn;

    [HideInInspector] public CommandType commandEnum;
    [HideInInspector] public int count;
    [HideInInspector] public int needTime;
    
    protected Action<PointerEventData> _scrollOnBeginDrag;
    protected Action<PointerEventData> _scrollOnDrag;
    protected Action<PointerEventData> _scrollOnEndDrag;

    protected UnityAction btnClickCallback;


    public void Init(CommandType commandEnumT, int countT,int needTimeT,UnityAction btnClickCallbackT
        , Action<PointerEventData> scrollOnBeginDrag, Action<PointerEventData> scrollOnDrag, Action<PointerEventData> scrollOnEndDrag)
    {
        commandEnum = commandEnumT;
        count = countT;
        needTime = needTimeT;
        btnClickCallback = btnClickCallbackT;
        transform.Find("Decorate").Find("ClickBtnBg").Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
        clickBtn.transform.Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
        clickBtn.transform.GetComponent<Button>().onClick.AddListener(btnClickCallback);
        _scrollOnBeginDrag = scrollOnBeginDrag;
        _scrollOnDrag = scrollOnDrag;
        _scrollOnEndDrag = scrollOnEndDrag;
    }
    

}