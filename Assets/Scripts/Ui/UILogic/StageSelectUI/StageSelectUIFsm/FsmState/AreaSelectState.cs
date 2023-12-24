using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelectState : IFsmComponentState<StageSelectUICtrl>
{
    public void OnEnter(StageSelectUICtrl owner)
    {
        owner.AreaSelectStateEnter();
    }

    public void OnUpdate(StageSelectUICtrl owner)
    {
        owner.AreaSelectStateUpdate();
    }

    public void OnExit(StageSelectUICtrl owner)
    {
        
    }
}
