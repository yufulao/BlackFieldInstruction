using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMainEndState : IFsmState
{
    public void OnEnter()
    {
        CommandManager.Instance.CommandMainEndStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
