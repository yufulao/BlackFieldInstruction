using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCalculateState : IFsmState
{
    public void OnEnter()
    {
        CommandManager.Instance.CommandCalculateStateEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
