using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandExecuteStartState : IFsmState
{
    public void OnEnter()
    {
        CommandManager.Instance.CommandExecuteStartStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
