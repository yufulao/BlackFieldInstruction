using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleUnit:MonoBehaviour
{
    public bool walkable;
    public UnitType unitType;

    /// <summary>
    /// 初始化时
    /// </summary>
    public virtual void OnUnitInit()
    {
        
    }

    /// <summary>
    /// 重置时
    /// </summary>
    public virtual void OnUnitReset()
    {

    }

    /// <summary>
    /// 销毁时
    /// </summary>
    public virtual void OnUnitDestroy()
    {
        
    }

    /// <summary>
    /// 计算下一个指令执行期间要做的事
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Calculate(CommandType commandType)
    {
        yield return null;
    }

    /// <summary>
    /// 执行完指令后检测受影响的unit
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator CheckOverlap()
    {
        yield return null;
    }

    /// <summary>
    /// unit的当前执行的指令
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Execute()
    {
        yield return null;
    }
}