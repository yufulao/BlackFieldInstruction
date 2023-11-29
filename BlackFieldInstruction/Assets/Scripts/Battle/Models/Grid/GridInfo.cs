using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]//后面改成cfg==========================================================================
public class GridObjInfo
{
    //配置变量
    public bool walkable;
    public GridObjType gridObjType;
    
    //动态变量
    [HideInInspector] public Vector2Int originalPoint;//初始网格坐标
    
}
