using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCheckOverlapState : IFsmState
{
    public void OnEnter()
    {
        CommandManager.Instance.CommandCheckOverlapStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
