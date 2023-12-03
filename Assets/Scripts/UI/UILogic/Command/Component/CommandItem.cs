using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandItem : MonoBehaviour
{
    [HideInInspector] public CommandType commandEnum;
    [HideInInspector] public int count;
    
    [HideInInspector]public Text countText;
    
    [HideInInspector]public GameObject clickBtnProto;
    [HideInInspector]public Transform clickBtnContainer;
    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector]public List<GameObject> btnList=new List<GameObject>();



    public void Init(CommandType commandEnumT, int countT)
    {
        commandEnum = commandEnumT;
        count = countT;
        clickBtnContainer = transform.Find("ClickBtnContainer");
        clickBtnProto = AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["ClickBtnProto"].prefabPath);
        canvasGroup = transform.GetOrAddComponent<CanvasGroup>();
    }
}
