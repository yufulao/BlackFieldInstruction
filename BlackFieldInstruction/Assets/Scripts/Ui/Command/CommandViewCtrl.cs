using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandViewCtrl : UiCtrlBase
{
    
    protected override void SetMvc()
    {
        base.SetMvc();
        _view = new CommandViewView();
        _model = new CommandViewModel();
    }

    
}
