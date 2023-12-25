using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowSelectState : IFsmComponentState<StageSelectUICtrl>
{
    public void OnEnter(StageSelectUICtrl owner)
    {
        owner.LowSelectStateEnter();
    }

    public void OnUpdate(StageSelectUICtrl owner)
    {
        owner.LowSelectStateUpdate();
    }

    public void OnExit(StageSelectUICtrl owner)
    {
        
    }
}
