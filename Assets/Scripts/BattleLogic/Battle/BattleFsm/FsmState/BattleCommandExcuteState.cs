using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleCommandExcuteState : IFsmState
{
    public void OnEnter()
    {
        BattleManager.Instance.BattleCommandExecuteStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
