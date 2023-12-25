using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndState : IFsmState
{
    public void OnEnter()
    {
        BattleManager.Instance.BattleEndStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
