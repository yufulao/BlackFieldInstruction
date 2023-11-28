using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public List<GridObject> gridObjList;

    public GridCell()
    {
        gridObjList = new List<GridObject>();
    }
    
    /// <summary>
    /// 检测格子是否可走
    /// </summary>
    /// <returns></returns>
    public bool CheckWalkable()
    {
        for (int i = 0; i < gridObjList.Count; i++)
        {
            if (!gridObjList[i].gridInfo.walkable)
            {
                return false;
            }
        }

        return true;
    }
}
