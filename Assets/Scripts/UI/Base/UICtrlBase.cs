using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UICtrlBase : MonoBehaviour
{
    public virtual void OnInit(params object[] param)
    {
    }

    public virtual void OpenRoot()
    {
    }

    public virtual void CloseRoot()
    {
    }
    
    protected virtual void BindEvent()
    {
    }
}