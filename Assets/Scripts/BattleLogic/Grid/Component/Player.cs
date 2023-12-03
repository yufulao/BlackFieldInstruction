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
    private ForwardType _originalForwardType;
    private Tweener _tweener;

    public override void OnInit()
    {
        base.OnInit();
        _originalForwardType = ForwardType.Up;
        _currentForwardType = _originalForwardType;
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    public override void OnReset()
    {
        base.OnReset();
        StartCoroutine(WaitForRotate(_originalForwardType));
    }

    /// <summary>
    /// player移动指令
    /// </summary>
    /// <param name="commandEnum">指令类型</param>
    /// <param name="during">移动时间</param>
    /// <returns></returns>
    public IEnumerator MoveCommand(CommandType commandEnum,float during=1f)
    {
        Vector2Int lastPoint=gridObjInfo.currentPoint;
        ForwardType targetForward = _currentForwardType;
        Vector2Int newPoint=lastPoint;
        switch (commandEnum)
        {
            case CommandType.Up:
                targetForward = ForwardType.Up;
                newPoint = new Vector2Int(lastPoint.x, lastPoint.y + 1);
                break;
            case CommandType.Down:
                targetForward = ForwardType.Down;
                newPoint = new Vector2Int(lastPoint.x, lastPoint.y-1);
                break;
            case CommandType.Left:
                targetForward = ForwardType.Left;
                newPoint = new Vector2Int(lastPoint.x-1, lastPoint.y);
                break;
            case CommandType.Right:
                targetForward = ForwardType.Right;
                newPoint = new Vector2Int(lastPoint.x+1, lastPoint.y);
                break;
            case CommandType.Wait:
                newPoint = new Vector2Int(lastPoint.x, lastPoint.y);
                break;
        }
        
        yield return StartCoroutine(WaitForRotate(targetForward));
        //Debug.Log(GridManager.Instance.CheckWalkable(targetPoint.x, targetPoint.y));
        // Debug.Log(newPoint.x+"  "+newPoint.y);
        if (GridManager.Instance.CheckWalkable(newPoint.x, newPoint.y))
        {
            _tweener=_rb.DOMove(GridManager.Instance.GetWorldPositionByPoint(newPoint.x, newPoint.y), during);
            //Debug.Log(GridManager.Instance.GetWorldPositionByPoint(targetPoint.x, targetPoint.y));
            yield return _tweener.WaitForCompletion();
            GridManager.Instance.UpdateGridObjPoint(this,lastPoint,newPoint);//更新GridObj
        }
        else
        {
            yield return new WaitForSeconds(during);
        }
        
        if (!GridManager.Instance.CheckPlayerGetTarget())
        {
            CommandManager.Instance.ExcuteCommand();
        }
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
                _tweener=transform.DORotate(new Vector3(0,0,0),during);
                break;
            case ForwardType.Down:
                _tweener=transform.DORotate(new Vector3(0,180,0),during);
                break;
            case ForwardType.Left:
                _tweener=transform.DORotate(new Vector3(0,-90,0),during);
                break;
            case ForwardType.Right:
                _tweener=transform.DORotate(new Vector3(0,90,0),during);
                break;
        }

        yield return _tweener.WaitForCompletion();
        _currentForwardType = targetForward;
    } 
    
}
