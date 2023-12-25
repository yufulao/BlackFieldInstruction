using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighToMidState : IFsmComponentState<StageSelectUICtrl>
{
    public void OnEnter(StageSelectUICtrl owner)
    {
        owner.HighToMidStateEnter();
    }

    public void OnUpdate(StageSelectUICtrl owner)
    {
        
    }

    public void OnExit(StageSelectUICtrl owner)
    {
        
    }
}
