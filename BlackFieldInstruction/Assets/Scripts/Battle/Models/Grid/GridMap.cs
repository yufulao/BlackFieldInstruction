using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class GridMap : MonoBehaviour
{
    private int _width;//x轴有几个格子
    private int _height;//z轴有几个格子
    private float _cellSize;//每个格子的宽度和长度
    private GridCell[,] _map;
    private List<GridObject> _allGridObjects;

    /// <summary>
    /// 初始化并创建网格地图
    /// </summary>
    /// <param name="width">网格坐标宽度</param>
    /// <param name="height">网格坐标高度</param>
    /// <param name="cellSize">单个格子的实际宽度，宽高相等</param>
    public void InitMap(int width,int height,float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _allGridObjects = new List<GridObject>();
        CreatMap();
        LoadGridObjs();
    }

    /// <summary>
    /// 重置网格位置
    /// </summary>
    public void ResetGridMap()
    {
        for (int i = 0; i < _allGridObjects.Count; i++)
        {
            _allGridObjects[i].OnReset();
        }

        CreatMap();
        var gridObjList= transform.GetComponentsInChildren<GridObject>();
        for (int i = 0; i < gridObjList.Length; i++)
        {
            _allGridObjects.Add(gridObjList[i]);
            Vector2Int objPoint= GetPointByWorldPosition(gridObjList[i].transform.position);
            gridObjList[i].gridObjInfo.originalPoint = objPoint;
            gridObjList[i].gridObjInfo.currentPoint = objPoint;
            _map[objPoint.x,objPoint.y].gridObjList.Add(gridObjList[i]);
        }
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
    /// 更新单个GridCell
    /// </summary>
    /// <param name="gridObject">需要更新的gridObject</param>
    /// <param name="lastPoint">更新之前的point坐标</param>
    /// <param name="newPoint">更新之后的point坐标</param>
    public void UpdateGridObjPoint(GridObject gridObject,Vector2Int lastPoint,Vector2Int newPoint)
    {
        gridObject.gridObjInfo.currentPoint = newPoint;
        _map[lastPoint.x, lastPoint.y].gridObjList.Remove(gridObject);
        _map[newPoint.x,newPoint.y].gridObjList.Add(gridObject);
    }
    
    /// <summary>
    /// 世界坐标转网格坐标
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector2Int GetPointByWorldPosition(Vector3 worldPosition)
    {
        //Debug.Log(new Vector2Int((int) (worldPosition.x / _cellSize), (int) (worldPosition.z / _cellSize)));
        return new Vector2Int((int) (worldPosition.x / _cellSize), (int) (worldPosition.z / _cellSize));
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
        return new Vector3(x * _cellSize,y, z * _cellSize);
    }
    
    /// <summary>
    /// 创建网格地图
    /// </summary>
    private void CreatMap()
    {
        _map = new GridCell[_width, _height];
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int z = 0; z < _map.GetLength(1); z++)
            {
                _map[x, z] = new GridCell(x,z);
            }
        }
    }

    /// <summary>
    /// 加载场景中的GridObject
    /// </summary>
    private void LoadGridObjs()
    {
        Player player = null;
        List<TargetObject> targetObjects = new List<TargetObject>();
        
        var gridObjList= transform.GetComponentsInChildren<GridObject>();
        for (int i = 0; i < gridObjList.Length; i++)
        {
            _allGridObjects.Add(gridObjList[i]);
            Vector2Int objPoint= GetPointByWorldPosition(gridObjList[i].transform.position);
            gridObjList[i].gridObjInfo.originalPoint = objPoint;
            gridObjList[i].gridObjInfo.currentPoint = objPoint;
            _map[objPoint.x,objPoint.y].gridObjList.Add(gridObjList[i]);
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

        GridManager.Instance.SetGridObjectsParams(player,targetObjects);
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
    
}