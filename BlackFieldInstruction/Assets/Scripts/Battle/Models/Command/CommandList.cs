using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandList
{
    public IEnumerator CommandUp(Player player)
    {
        yield return player.MoveCommand(CommandEnum.Up);
    }
    
    public IEnumerator CommandDown(Player player)
    {
        yield return player.MoveCommand(CommandEnum.Down);
    }
    
    public IEnumerator CommandLeft(Player player)
    {
        yield return player.MoveCommand(CommandEnum.Left);
    }
    
    public IEnumerator CommandRight(Player player)
    {
        yield return player.MoveCommand(CommandEnum.Right);
    }
    
    public IEnumerator CommandWait(Player player)
    {
        yield return player.MoveCommand(CommandEnum.Wait);
    }
}
