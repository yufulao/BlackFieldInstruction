using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    public bool debugShow;//是否显示辅助线
    private GridMap _gridMap;//动态的，会切换
    private GridMapView _gridMapView;

    /// <summary>
    /// 重新获取场景中的girdModel和gridView
    /// </summary>
    public void LoadGridManager(RowCfgStage rowCfgStage)
    {
        _gridMap =GameObject.FindObjectOfType<GridMap>();
        _gridMapView = GameObject.FindObjectOfType<GridMapView>();
        _gridMap.InitMap(rowCfgStage.width,rowCfgStage.height,rowCfgStage.cellSize);
        if (debugShow)
        {
            _gridMapView.ShowDebug(rowCfgStage.width,rowCfgStage.height,rowCfgStage.cellSize);
        }
    }

    /// <summary>
    /// 世界坐标转网格坐标
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector2Int GetPointByWorldPosition(Vector3 worldPosition)
    {
        return _gridMap.GetPointByWorldPosition(worldPosition);
    }
    
    /// <summary>
    /// 网格坐标转世界坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetWorldPositionByPoint(int x,int z,float y=0)
    {
        return _gridMap.GetWorldPositionByPoint(x,z,y);
    }

    /// <summary>
    /// 检测[x,z]是否可以走
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool CheckWalkable(int x,int z)
    {
        return _gridMap.CheckCanWalk(x, z);
    }
}
