using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMap : MonoBehaviour
{
    private int _width;//x轴有几个格子
    private int _height;//z轴有几个格子
    private float _cellSize;//每个格子的宽度和长度
    private GridCell[,] _map;
    private Player _player;

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
        CreatMap();
        LoadGridObjs();
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
        var gridObjList= transform.GetComponentsInChildren<GridObject>();
        for (int i = 0; i < gridObjList.Length; i++)
        {
            Vector2Int objPoint= GetPointByWorldPosition(gridObjList[i].transform.position);
            gridObjList[i].gridObjInfo.originalPoint = objPoint;
            _map[objPoint.x,objPoint.y].gridObjList.Add(gridObjList[i]);
        }
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