using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public List<GridObject> gridObjList;
    private int _x;
    private int _z;

    /// <summary>
    /// 生成cell并设置坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public GridCell(int x,int z)
    {
        gridObjList = new List<GridObject>();
        _x = x;
        _z = z;
    }

    /// <summary>
    /// 获取当前坐标
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetPoint()
    {
        return new Vector2Int(_x, _z);
    }
    
    /// <summary>
    /// 检测格子是否可走
    /// </summary>
    /// <returns></returns>
    public bool CheckWalkable()
    {
        for (int i = 0; i < gridObjList.Count; i++)
        {
            if (!gridObjList[i].gridObjInfo.walkable)
            {
                return false;
            }
        }

        return true;
    }
}
