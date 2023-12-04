using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandItem : MonoBehaviour
{
    [SerializeField]protected Text countText;
    [SerializeField]protected CanvasGroup canvasGroup;
    [SerializeField]protected Button clickBtn;

    [HideInInspector] public CommandType commandEnum;
    [HideInInspector] public int count;
    [HideInInspector] public int needTime;


    public void Init(CommandType commandEnumT, int countT,int needTimeT)
    {
        commandEnum = commandEnumT;
        count = countT;
        needTime = needTimeT;
        transform.Find("Decorate").Find("ClickBtnBg").Find("Text (Legacy)").GetComponent<Text>().text = commandEnum.ToString();
    }
}