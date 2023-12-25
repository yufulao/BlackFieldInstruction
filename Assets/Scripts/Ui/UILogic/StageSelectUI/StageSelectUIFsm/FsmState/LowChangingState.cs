using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowChangingState : IFsmComponentState<StageSelectUICtrl>
{
    public void OnEnter(StageSelectUICtrl owner)
    {
        owner.LowChangingStateEnter();
    }

    public void OnUpdate(StageSelectUICtrl owner)
    {
        
    }

    public void OnExit(StageSelectUICtrl owner)
    {
        
    }
}
