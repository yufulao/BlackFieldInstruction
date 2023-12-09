using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StageItem : MonoBehaviour
{
    [SerializeField] private Button clickBtn;
    [SerializeField] private Text stageNameText;

    public void SetClickBtnOnClick(UnityAction action)
    {
        clickBtn.onClick.AddListener(action);
    }

    public void Refresh(string stageName)
    {
        stageNameText.text = stageName;
    }
}
