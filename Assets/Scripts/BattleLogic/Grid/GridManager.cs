using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class GridManager : BaseSingleTon<GridManager>
{
    private int _width;//x轴有几个格子
    private int _height;//z轴有几个格子
    public bool debugShow;//是否显示辅助线
    private GridCell[,] _map;
    private Dictionary<int[,], GridCell> _gridCellDic = new Dictionary<int[,], GridCell>();
    [HideInInspector]public Player player;
    private List<TargetObject> _targetObjects=new List<TargetObject>();
    private readonly List<GridObject> _allGridObjects= new List<GridObject>();
    private GridModel _gridModel;//动态的，会切换
    private GridView _gridView;


    /// <summary>
    /// 重新获取场景中的girdModel和gridView
    /// </summary>
    public void LoadGridManager(RowCfgStage rowCfgStage)
    {
        _gridModel =new GridModel();
        _gridView = GameObject.FindObjectOfType<GridView>();
        _width = rowCfgStage.width;
        _height = rowCfgStage.height;
        _gridModel.InitCellSize(rowCfgStage.cellSize);
        CreatMap();
        LoadGridObjs();

        if (debugShow)
        {
            _gridView.ShowDebug(rowCfgStage.width,rowCfgStage.height,rowCfgStage.cellSize);
        }
    }

    /// <summary>
    /// 重置网格地图
    /// </summary>
    public void ResetGridManager()
    {
        for (int i = 0; i < _allGridObjects.Count; i++)
        {
            _allGridObjects[i].OnReset();
        }
        CreatMap();
        var gridObjList= _gridView.GetGridObjContainer().GetComponentsInChildren<GridObject>();
        for (int i = 0; i < gridObjList.Length; i++)
        {
            _allGridObjects.Add(gridObjList[i]);
            Vector2Int objPoint= _gridModel.GetPointByWorldPosition(gridObjList[i].transform.position);
            gridObjList[i].gridObjInfo.originalPoint = objPoint;
            gridObjList[i].gridObjInfo.currentPoint = objPoint;
            AddGridCellObj(objPoint.x,objPoint.y,gridObjList[i]);
        }
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
    /// 更新单个GridCell
    /// </summary>
    /// <param name="gridObject">需要更新的gridObject</param>
    /// <param name="lastPoint">更新之前的point坐标</param>
    /// <param name="newPoint">更新之后的point坐标</param>
    public void UpdateGridObjPoint(GridObject gridObject,Vector2Int lastPoint,Vector2Int newPoint)
    {
        gridObject.gridObjInfo.currentPoint = newPoint;
        RemoveGridCellObj(lastPoint.x, lastPoint.y,gridObject);
        AddGridCellObj(newPoint.x,newPoint.y,gridObject);
    }

    /// <summary>
    /// 设置GridManager的GridObject参数，_gridModel.InitMap()中调用
    /// </summary>
    /// <param name="playerT"></param>
    /// <param name="targetObjects"></param>
    private void SetGridObjectsParams(Player playerT,List<TargetObject> targetObjects)
    {
        player = playerT;
        _targetObjects = targetObjects;
    }
    
    /// <summary>
    /// 加载场景中的GridObject
    /// </summary>
    private void LoadGridObjs()
    {
        Player player = null;
        List<TargetObject> targetObjects = new List<TargetObject>();
        
        var gridObjList= _gridView.GetGridObjContainer().GetComponentsInChildren<GridObject>();
        for (int i = 0; i < gridObjList.Length; i++)
        {
            _allGridObjects.Add(gridObjList[i]);
            Vector2Int objPoint= _gridModel.GetPointByWorldPosition(gridObjList[i].transform.position);
            gridObjList[i].gridObjInfo.originalPoint = objPoint;
            gridObjList[i].gridObjInfo.currentPoint = objPoint;
            AddGridCellObj(objPoint.x,objPoint.y,gridObjList[i]);
            gridObjList[i].OnInit();
            
            if (gridObjList[i] is Player)
            {
                player = gridObjList[i] as Player;
            }

            if (gridObjList[i] is TargetObject)
            {
                targetObjects.Add(gridObjList[i] as  TargetObject);
            }
        }

        SetGridObjectsParams(player,targetObjects);
    }
    
    private void AddGridCellObj(int x,int z,GridObject gridObject)
    {
        _map[x,z].gridObjList.Add(gridObject);
    }

    private void RemoveGridCellObj(int x,int z,GridObject gridObject)
    {
        _map[x,z].gridObjList.Remove(gridObject);
    }
    
    /// <summary>
    /// 检测该格子是否可以走
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool CheckCanWalk(int x,int z)
    {
        if (!CheckPointValid(x,z))
            return false;
        
        return _map[x,z].CheckWalkable();
    }
    
    /// <summary>
    /// 检测网格坐标是否在网格范围内
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private bool CheckPointValid(int x,int z)
    {
        if (_map==null)
        {
            return false;
        }
        return x >= 0 && z >= 0 && x < _map.GetLength(0) && z < _map.GetLength(1);
    }
    
    /// <summary>
    /// 创建网格地图
    /// </summary>
    private void CreatMap()
    {
        _map = new GridCell[_width,_height];
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int z = 0; z < _map.GetLength(1); z++)
            {
                _map[x, z] = new GridCell(x,z);
            }
        }
    }
}
