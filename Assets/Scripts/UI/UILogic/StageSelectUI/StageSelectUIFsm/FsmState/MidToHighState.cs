using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidToHighState : IFsmComponentState<StageSelectUICtrl>
{
    public void OnEnter(StageSelectUICtrl owner)
    {
        owner.MidToHighStateEnter();
    }

    public void OnUpdate(StageSelectUICtrl owner)
    {
        
    }

    public void OnExit(StageSelectUICtrl owner)
    {
        
    }
}
