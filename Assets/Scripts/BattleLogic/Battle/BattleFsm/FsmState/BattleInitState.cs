using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitState : IFsmState
{
    public void OnEnter()
    {
        BattleManager.Instance.BattleInitStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
