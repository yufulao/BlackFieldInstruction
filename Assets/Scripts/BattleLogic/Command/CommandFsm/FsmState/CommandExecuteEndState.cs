using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandExecuteEndState : IFsmState
{
    public void OnEnter()
    {
        CommandManager.Instance.CommandExecuteEndStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
