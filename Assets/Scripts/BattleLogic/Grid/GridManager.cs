using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    public bool debugShow;//是否显示辅助线
    [HideInInspector]public Player player;
    private List<TargetObject> _targetObjects;
    private GridModel _gridModel;//动态的，会切换
    private GridView _gridView;

    /// <summary>
    /// 重新获取场景中的girdModel和gridView
    /// </summary>
    public void LoadGridManager(RowCfgStage rowCfgStage)
    {
        _gridModel =GameObject.FindObjectOfType<GridModel>();
        _gridView = GameObject.FindObjectOfType<GridView>();
        _gridModel.InitMap(rowCfgStage.width,rowCfgStage.height,rowCfgStage.cellSize);
        
        if (debugShow)
        {
            _gridView.ShowDebug(rowCfgStage.width,rowCfgStage.height,rowCfgStage.cellSize);
        }
    }

    /// <summary>
    /// 重置网格地图
    /// </summary>
    public void ResetGridModel()
    {
        _gridModel.ResetGridModel();
    }

    /// <summary>
    /// 更新单个GridCell
    /// </summary>
    /// <param name="gridObject">需要更新的gridObject</param>
    /// <param name="lastPoint">更新之前的point坐标</param>
    /// <param name="newPoint">更新之后的point坐标</param>
    public void UpdateGridObjPoint(GridObject gridObject,Vector2Int lastPoint,Vector2Int newPoint)
    {
        _gridModel.UpdateGridObjPoint(gridObject, lastPoint, newPoint);
    }
    
    /// <summary>
    /// 设置GridManager的GridObject参数，_gridModel.InitMap()中调用
    /// </summary>
    /// <param name="playerT"></param>
    /// <param name="targetObjects"></param>
    public void SetGridObjectsParams(Player playerT,List<TargetObject> targetObjects)
    {
        player = playerT;
        _targetObjects = targetObjects;
    }

    /// <summary>
    /// player每次Move之后检测是否抵达目标，以及当指令结束完毕时再调用一次计算本轮最终结果
    /// </summary>
    public bool CheckPlayerGetTarget()
    {
        for (int i = 0; i < _targetObjects.Count; i++)
        {
            if (player.gridObjInfo.currentPoint==_targetObjects[i].gridObjInfo.currentPoint)
            {
                BattleManager.Instance.BattleEnd(true);
                return true;
            }
        }
        return false;
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
    /// 检测[x,z]是否可以走
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool CheckWalkable(int x,int z)
    {
        return _gridModel.CheckCanWalk(x, z);
    }
    
}
