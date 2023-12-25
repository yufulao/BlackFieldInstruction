using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowToMidState : IFsmComponentState<StageSelectUICtrl>
{
    public void OnEnter(StageSelectUICtrl owner)
    {
        owner.LowToMidStateEnter();
    }

    public void OnUpdate(StageSelectUICtrl owner)
    {
        
    }

    public void OnExit(StageSelectUICtrl owner)
    {
        
    }
}
