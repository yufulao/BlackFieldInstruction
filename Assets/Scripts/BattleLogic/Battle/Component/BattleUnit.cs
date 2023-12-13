using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleUnit:MonoBehaviour
{
    public bool walkable;
    public UnitType unitType;


    public virtual void OnUnitInit()
    {
        
    }

    public virtual void OnUnitReset()
    {

    }

    public virtual void OnUnitDestroy()
    {
        
    }

}