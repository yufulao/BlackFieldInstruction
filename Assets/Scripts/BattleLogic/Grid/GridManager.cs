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

    public Transform GetGridObjContainer()
    {
        return _gridView.GetGridObjContainer();
    }

    public GridCell GetGridCell(int x,int z)
    {
         return _gridModel.GetGridCell(x, z);
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

    public Vector2Int GetPointByWorldPosition(Vector3 worldPosition)
    {
        return _gridModel.GetPointByWorldPosition(worldPosition);
    }

    public bool CheckPointValid(int x,int z)
    {
        return _gridModel.CheckPointValid(x, z);
    }
}
