using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface UICtrlBase
{
    void OnInit(params object[] param);

    void BindEvent();

    void OpenRoot();

    void CloseRoot();
}
