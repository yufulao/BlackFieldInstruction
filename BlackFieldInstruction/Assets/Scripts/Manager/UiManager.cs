using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class UiManager : BaseSingleTon<UiManager>,IMonoManager
{
    private CfgUi _cfgUi;
    private Transform uiRoot;
    private Transform sceneLayer;
    private Transform normalLayer;
    private Transform topLayer;
    private Dictionary<string, Transform> _layers;
    private Dictionary<string, UiCtrlBase> _allViews;
    private Dictionary<string, Stack<UiCtrlBase>> _layerStacks;


    public void OnInit()
    {
        _cfgUi = ConfigManager.Instance.cfgUi;
        
        uiRoot = GameObject.Find("UiRoot").transform;

        _layers = new Dictionary<string, Transform>();
        _layers.Add("SceneLayer",GameObject.Find("SceneLayer").transform);
        _layers.Add("NormalLayer",GameObject.Find("NormalLayer").transform);
        _layers.Add("TopLayer",GameObject.Find("TopLayer").transform);

        _layerStacks = new Dictionary<string, Stack<UiCtrlBase>>();
        _layerStacks.Add("SceneLayer",new Stack<UiCtrlBase>());
        _layerStacks.Add("NormalLayer",new Stack<UiCtrlBase>());
        _layerStacks.Add("TopLayer",new Stack<UiCtrlBase>());
        
        _allViews = new Dictionary<string, UiCtrlBase>();
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
    public IEnumerator OpenWindow(string windowName,Action<UiCtrlBase> callback=null)
    {
        yield return GetCtrl(windowName, (ctrl) =>
        {
            _layerStacks[ConfigManager.Instance.cfgUi[windowName].layer].Push(ctrl);
            ctrl.OpenRoot();
            callback?.Invoke(ctrl);
        });
    }
    
    /// <summary>
    /// 关闭页面
    /// </summary>
    /// <param name="windowName"></param>
    public IEnumerator CloseWindows(string windowName)
    {
        yield return GetCtrl(windowName, (ctrl) =>
        {
            string layer = ConfigManager.Instance.cfgUi[windowName].layer;
            while (_layerStacks[layer].Count!=0)
            {
                UiCtrlBase ctrlBefore= _layerStacks[layer].Pop();
                if (ctrlBefore==ctrl)
                {
                    break;
                }
                ctrlBefore.CloseRoot();
            }
            ctrl.CloseRoot();
        });
    }

    private IEnumerator GetCtrl(string windowName,Action<UiCtrlBase> callback)
    {
        if (_allViews.ContainsKey(windowName))
        {
            callback?.Invoke(_allViews[windowName]);
            yield break;
        }

        yield return CreatNewView(windowName, (ctrl) =>
        {
            callback?.Invoke(ctrl);
        });
    }

    private IEnumerator CreatNewView(string windowName ,Action<UiCtrlBase> callback)
    {
        RowCfgUi rowCfgUi = _cfgUi[windowName];
        yield return AssetManager.Instance.LoadAssetAsync<GameObject>(rowCfgUi.uiPath, (obj) =>
        {
            GameObject rootObj = GameObject.Instantiate(obj, _layers[rowCfgUi.layer]);
            //rootObj上的ctrl开始start并实例化view和model
            rootObj.SetActive(false);
            rootObj.GetComponent<Canvas>().sortingOrder = rowCfgUi.sortOrder;
            
            UiCtrlBase ctrlNew=null;
            Component[] components = rootObj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is UiCtrlBase)
                {
                    ctrlNew=components[i] as UiCtrlBase;
                    break;
                }
            }
            
            if (ctrlNew==null)
            {
                Debug.LogError("找不到viewObj挂载的ctrl"+rootObj.name);
                return;
            }
            
            _allViews.Add(windowName,ctrlNew);
            ctrlNew.OnInit(rootObj.transform);
            callback?.Invoke(ctrlNew);
        });
        
    }

}
