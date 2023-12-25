using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandExecutingState : IFsmState
{
    public void OnEnter()
    {
        CommandManager.Instance.CommandExecutingStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
