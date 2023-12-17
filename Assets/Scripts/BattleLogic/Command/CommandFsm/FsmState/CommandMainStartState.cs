using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMainStartState : IFsmState
{
    public void OnEnter()
    {
        CommandManager.Instance.CommandMainStartStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
