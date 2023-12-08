using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]//后面改成cfg==========================================================================
public class BattleUnitInfo
{
    //动态变量
    [HideInInspector]public Vector2Int originalPoint;//初始网格坐标
    [HideInInspector]public Vector2Int currentPoint;

}
