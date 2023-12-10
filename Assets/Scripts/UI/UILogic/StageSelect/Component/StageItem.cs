using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StageItem : MonoBehaviour
{
    [SerializeField] private Button clickBtn;
    [SerializeField] private Text stageNameText;

    /// <summary>
    /// 设置item的点击事件
    /// </summary>
    /// <param name="action"></param>
    public void SetClickBtnOnClick(UnityAction action)
    {
        clickBtn.onClick.AddListener(action);
    }

    /// <summary>
    /// 刷新item的StageName显示
    /// </summary>
    /// <param name="stageName"></param>
    public void Refresh(string stageName)
    {
        stageNameText.text = stageName;
    }
}
