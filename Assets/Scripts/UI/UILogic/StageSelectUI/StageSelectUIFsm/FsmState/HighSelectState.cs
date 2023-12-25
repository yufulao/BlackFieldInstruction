using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSelectState : IFsmComponentState<StageSelectUICtrl>
{
    public void OnEnter(StageSelectUICtrl owner)
    {
        owner.HighSelectStateEnter();
    }

    public void OnUpdate(StageSelectUICtrl owner)
    {
        
    }

    public void OnExit(StageSelectUICtrl owner)
    {
        
    }
}
