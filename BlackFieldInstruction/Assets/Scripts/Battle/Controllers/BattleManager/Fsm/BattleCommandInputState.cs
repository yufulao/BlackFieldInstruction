using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCommandInputState : IFsmState
{
    public void OnEnter()
    {
        
    }

    public void OnUpdate()
    {
        BattleManager.Instance.BattleInputStateUpdate();
    }

    public void OnExit()
    {
        
    }
}
