using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : GridObject
{
    private Rigidbody _rb;
    private Animator _animator;
    private ForwardType _currentForwardType;
    private GridCell _currentCell;

    /// <summary>
    /// player移动指令
    /// </summary>
    /// <param name="commandEnum">指令类型</param>
    /// <param name="callback">返回是否执行成功</param>
    /// <param name="during">移动时间</param>
    /// <returns></returns>
    public IEnumerator MoveCommand(CommandEnum commandEnum,Action<bool> callback=null,float during=1f)
    {
        Vector2Int point = _currentCell.GetPoint();
        Vector2Int targetPoint = point;
        switch (commandEnum)
        {
            case CommandEnum.Up:
                targetPoint = new Vector2Int(point.x, point.y + 1);
                break;
            case CommandEnum.Down:
                targetPoint = new Vector2Int(point.x, point.y-1);
                break;
            case CommandEnum.Left:
                targetPoint = new Vector2Int(point.x-1, point.y);
                break;
            case CommandEnum.Right:
                targetPoint = new Vector2Int(point.x+1, point.y);
                break;
            case CommandEnum.Wait:
                break;
        }

        if (GridManager.Instance.CheckWalkable(targetPoint.x, targetPoint.y))
        {
            _rb.DOMove(GridManager.Instance.GetWorldPositionByPoint(targetPoint.x, targetPoint.y), during);
            yield return during;
            callback?.Invoke(true);
            yield break;
        }
        
        callback?.Invoke(false);
    }
    
    /// <summary>
    /// 旋转到目标方向
    /// </summary>
    /// <param name="targetForward">目标方向</param>
    /// <param name="during">旋转时间</param>
    /// <returns></returns>
    private IEnumerator WaitForRotate(ForwardType targetForward,float during=0.5f)
    {
        if (_currentForwardType==targetForward)
        {
            yield break;
        }
        switch (targetForward)
        {
            case ForwardType.Up:
                transform.DOLookAt(Vector3.forward,during);
                break;
            case ForwardType.Down:
                transform.DOLookAt(Vector3.back,during);
                break;
            case ForwardType.Left:
                transform.DOLookAt(Vector3.left,during);
                break;
            case ForwardType.Right:
                transform.DOLookAt(Vector3.right,during);
                break;
        }

        yield return during;
        _currentForwardType = targetForward;
    } 
    
}
