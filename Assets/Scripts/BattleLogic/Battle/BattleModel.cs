using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleModel
{
    private readonly Dictionary<GridCell, List<BattleUnitInfo>> _cellUnitInfoDic = new Dictionary<GridCell, List<BattleUnitInfo>>();

    /// <summary>
    /// 更新单个GridCell
    /// </summary>
    /// <param name="unitInfo">需要更新的gridObject</param>
    /// <param name="lastPoint">更新之前的point坐标</param>
    /// <param name="newPoint">更新之后的point坐标</param>
    public void UpdateUnitPoint(BattleUnitInfo unitInfo, Vector2Int lastPoint, Vector2Int newPoint)
    {
        unitInfo.currentPoint = newPoint;
        RemoveGridCellUnitInfo(lastPoint.x, lastPoint.y, unitInfo);
        AddGridCellUnitInfo(newPoint.x, newPoint.y, unitInfo);
    }

    /// <summary>
    /// 重置unitInfo
    /// </summary>
    /// <param name="unitInfo"></param>
    public void ResetUnitInfo(BattleUnitInfo unitInfo)
    {
        UpdateUnitPoint(unitInfo, unitInfo.currentPoint, unitInfo.originalPoint);
        unitInfo.currentPoint = unitInfo.originalPoint;
    }

    /// <summary>
    /// 是否创建了这个地块，没创建，这个地块就绝对没东西
    /// </summary>
    /// <param name="gridCell"></param>
    /// <returns></returns>
    public bool IsContainsGridCell(GridCell gridCell)
    {
        return _cellUnitInfoDic.ContainsKey(gridCell);
    }

    /// <summary>
    /// 获取这个地块上的所有unitInfo
    /// </summary>
    /// <param name="gridCell"></param>
    /// <returns></returns>
    public List<BattleUnitInfo> GetUnitInfoByGridCell(GridCell gridCell)
    {
        if (IsContainsGridCell(gridCell))
        {
            return _cellUnitInfoDic[gridCell];
        }

        return null;
    }

    /// <summary>
    /// 创建一个UnitInfo
    /// </summary>
    /// <param name="unitTypeT"></param>
    /// <param name="objPoint"></param>
    /// <returns></returns>
    public BattleUnitInfo CreatUnitInfo(UnitType unitTypeT,Vector2Int objPoint)
    {
        return new BattleUnitInfo()
        {
            unitType = unitTypeT,
            currentPoint = objPoint,
            originalPoint = objPoint
        };
    }

    /// <summary>
    /// 两个UnitInfo是否在同一个地块上
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool IsSameCurrentPoint(BattleUnitInfo a, BattleUnitInfo b)
    {
        return a.currentPoint == b.currentPoint;
    }

    /// <summary>
    /// 往一个地块里添加unitInfo
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="unitInfo"></param>
    public void AddGridCellUnitInfo(int x, int z, BattleUnitInfo unitInfo)
    {
        GridCell gridCell = GridManager.Instance.GetGridCell(x, z);
        if (!_cellUnitInfoDic.ContainsKey(gridCell))
        {
            //Debug.Log("AddCell:"+gridCell.GetPoint());
            _cellUnitInfoDic.Add(gridCell, new List<BattleUnitInfo>());
        }

        _cellUnitInfoDic[gridCell].Add(unitInfo);
    }

    /// <summary>
    /// 往一个地块里删除UnitInfo
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="unitInfo"></param>
    public void RemoveGridCellUnitInfo(int x, int z, BattleUnitInfo unitInfo)
    {
        GridCell gridCell = GridManager.Instance.GetGridCell(x, z);
        //Debug.Log("RemoveCell:"+gridCell.GetPoint());
        if (!_cellUnitInfoDic.ContainsKey(gridCell))
        {
            Debug.LogError("这个地块从来没有创建过，但想移除这个地块上的unitInfo" + gridCell.GetPoint());
        }

        _cellUnitInfoDic[gridCell].Remove(unitInfo);
    }

    /// <summary>
    /// 这个unitInfo是否不在初始点位
    /// </summary>
    /// <param name="unitInfo"></param>
    /// <returns></returns>
    public bool IsNoMove(BattleUnitInfo unitInfo)
    {
        return unitInfo.currentPoint == unitInfo.originalPoint;
    }
}