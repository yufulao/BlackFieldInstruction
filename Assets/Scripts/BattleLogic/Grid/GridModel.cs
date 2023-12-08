using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class GridModel
{
    private float _cellSize;//每个格子的宽度和长度

    /// <summary>
    /// 初始化并创建网格地图
    /// </summary>
    /// <param name="cellSize">单个格子的实际宽度，宽高相等</param>
    public void InitCellSize(float cellSize)
    {
        _cellSize = cellSize;
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
    public Vector3 GetWorldPositionByPoint(int x,int z,float y=0)
    {
        return new Vector3(x * _cellSize,y, z * _cellSize);
    }
}