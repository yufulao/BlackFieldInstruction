using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class GridModel
{
    private GridCell[,] _map;
    private int _width; //x轴有几个格子
    private int _height; //z轴有几个格子
    private float _cellSize; //每个格子的宽度和长度

    /// <summary>
    /// 初始化并创建网格地图
    /// </summary>
    /// <param name="rowCfgStage">单个格子的实际宽度，宽高相等</param>
    public void Init(RowCfgStage rowCfgStage)
    {
        _cellSize = rowCfgStage.cellSize;
        _width = rowCfgStage.width;
        _height = rowCfgStage.height;
        CreatMap();
    }

    public float GetPerCellSize()
    {
        return _cellSize;
    }

    /// <summary>
    /// 世界坐标转网格坐标
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector2Int GetPointByWorldPosition(Vector3 worldPosition)
    {
        // Debug.Log(worldPosition);
        // Debug.Log(_cellSize);
        // Debug.Log(new Vector2Int((int) (worldPosition.x / _cellSize), (int) (worldPosition.z / _cellSize)));
        return new Vector2Int((int) (worldPosition.x / _cellSize), (int) (worldPosition.z / _cellSize));
    }

    /// <summary>
    /// 网格坐标转世界坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetWorldPositionByPoint(int x, int z, float y = 0)
    {
        return new Vector3(x * _cellSize, y, z * _cellSize);
    }

    public GridCell GetGridCell(int x, int z)
    {
        return _map[x, z];
    }
    
    /// <summary>
    /// 检测网格坐标是否在网格范围内
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool CheckPointValid(int x,int z)
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