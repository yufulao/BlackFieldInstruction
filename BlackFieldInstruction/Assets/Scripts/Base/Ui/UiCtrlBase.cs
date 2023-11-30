using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiCtrlBase : MonoBehaviour
{
    [HideInInspector]
    public Transform root;
    protected UiViewBase _view;
    protected UiModelBase _model;
    
    public void OnInit(Transform viewRoot)
    {
        root = viewRoot;
        SetMvc();
        _view.OnInit();
        _model.OnInit();
        BindEvent();
    }

    protected virtual void SetMvc()
    {

    }

    public virtual void BindEvent()
    {
        
    }

    public virtual void OpenRoot()
    {
        root.gameObject.SetActive(true);
    }

    public virtual void CloseRoot()
    {
        root.gameObject.SetActive(false);
    }
}
