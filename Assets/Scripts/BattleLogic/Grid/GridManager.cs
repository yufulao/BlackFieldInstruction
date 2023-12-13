using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class GridManager : BaseSingleTon<GridManager>
{
    // private readonly List<BattleUnit> _allBattleUnits= new List<BattleUnit>();
    private GridModel _gridModel;//动态的，会切换
    private GridView _gridView;


    /// <summary>
    /// 重新获取场景中的girdModel和gridView
    /// </summary>
    public void InitGridManager(RowCfgStage rowCfgStage)
    {
        _gridModel =new GridModel();
        _gridView = GameObject.FindObjectOfType<GridView>();
        _gridModel.Init(rowCfgStage);

        if (_gridView.debugShow)
        {
            _gridView.ShowDebug(rowCfgStage.width,rowCfgStage.height,rowCfgStage.cellSize);
        }
    }

    /// <summary>
    /// 获取gridObj的容器
    /// </summary>
    /// <returns></returns>
    public Transform GetGridObjContainer()
    {
        return _gridView.GetGridObjContainer();
    }

    /// <summary>
    /// 通过坐标获取gridCell
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public GridCell GetGridCell(int x,int z)
    {
         return _gridModel.GetGridCell(x, z);
    }
    public GridCell GetGridCell(Vector2Int vector2Int)
    {
        return _gridModel.GetGridCell(vector2Int.x, vector2Int.y);
    }
    
    /// <summary>
    /// 获取每格的长宽
    /// </summary>
    /// <returns></returns>
    public float GetPerCellSize()
    {
        return _gridModel.GetPerCellSize();
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
        return _gridModel.GetWorldPositionByPoint(x,z,y);
    }

    /// <summary>
    /// 世界坐标转网格坐标
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector2Int GetPointByWorldPosition(Vector3 worldPosition)
    {
        return _gridModel.GetPointByWorldPosition(worldPosition);
    }

    /// <summary>
    /// 检测xz坐标是否在网格内
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool CheckPointValid(int x,int z)
    {
        return _gridModel.CheckPointValid(x, z);
    }
}
