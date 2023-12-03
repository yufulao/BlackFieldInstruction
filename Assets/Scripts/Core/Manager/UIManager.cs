using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class UIManager : BaseSingleTon<UIManager>,IMonoManager
{
    private CfgUI _cfgUI;
    private Transform _uiRoot;
    private Dictionary<string, Transform> _layers;
    private Dictionary<string, UICtrlBase> _allViews;
    private Dictionary<string, Stack<UICtrlBase>> _layerStacks;


    public void OnInit()
    {
        _cfgUI = ConfigManager.Instance.cfgUI;
        
        _uiRoot = GameObject.Find("UiRoot").transform;

        _layers = new Dictionary<string, Transform>();
        _layers.Add("SceneLayer",GameObject.Find("SceneLayer").transform);
        _layers.Add("NormalLayer",GameObject.Find("NormalLayer").transform);
        _layers.Add("TopLayer",GameObject.Find("TopLayer").transform);

        _layerStacks = new Dictionary<string, Stack<UICtrlBase>>();
        _layerStacks.Add("SceneLayer",new Stack<UICtrlBase>());
        _layerStacks.Add("NormalLayer",new Stack<UICtrlBase>());
        _layerStacks.Add("TopLayer",new Stack<UICtrlBase>());
        
        _allViews = new Dictionary<string, UICtrlBase>();
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        
    }

    public void LateUpdate()
    {
        
    }

    public void OnClear()
    {
        
    }
    
    /// <summary>
    /// 打开页面
    /// </summary>
    /// <param name="windowName"></param>
    /// <param name="callback"></param>
    public UICtrlBase OpenWindow(string windowName)
    {
        UICtrlBase ctrl = GetCtrl(windowName);

        _layerStacks[ConfigManager.Instance.cfgUI[windowName].layer].Push(ctrl);
        ctrl.OpenRoot();
        return ctrl;
    }
    
    /// <summary>
    /// 关闭页面
    /// </summary>
    /// <param name="windowName"></param>
    public void CloseWindows(string windowName)
    {
        UICtrlBase ctrl = GetCtrl(windowName); 
        string layer = ConfigManager.Instance.cfgUI[windowName].layer;
        while (_layerStacks[layer].Count!=0)
        {
            UICtrlBase ctrlBefore= _layerStacks[layer].Pop();
            if (ctrlBefore==ctrl)
            {
                break;
            }
            ctrlBefore.CloseRoot();
        }
        ctrl.CloseRoot();
    }

    public UICtrlBase GetCtrl(string windowName,params object[] param)
    {
        if (_allViews.ContainsKey(windowName))
        {
            return _allViews[windowName];
        }

        return CreatNewView(windowName,param);
    }

    private UICtrlBase CreatNewView(string windowName,params object[] param)
    {
        RowCfgUI rowCfgUi = _cfgUI[windowName];
        GameObject rootObj = GameObject.Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgUi.uiPath), _layers[rowCfgUi.layer]);
        
        //rootObj上的ctrl开始start并实例化view和model
        rootObj.SetActive(false);
        rootObj.GetComponent<Canvas>().sortingOrder = rowCfgUi.sortOrder;
            
        UICtrlBase ctrlNew=null;
        Component[] components = rootObj.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] is UICtrlBase)
            {
                ctrlNew=components[i] as UICtrlBase;
                break;
            }
        }
            
        if (ctrlNew==null)
        {
            Debug.LogError("找不到viewObj挂载的ctrl"+rootObj.name);
            return null;
        }
            
        _allViews.Add(windowName,ctrlNew);
        ctrlNew.OnInit(param);
        return ctrlNew;
    }

}
