using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleUnitPlayer : BattleUnit
{
    [SerializeField]private Rigidbody _rb;
    [SerializeField]private Animator _animator;
    [SerializeField]private ForwardType _originalForwardType= ForwardType.Up;
    private ForwardType _currentForwardType;
    private Sequence _sequence;

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        StartCoroutine(WaitForRotate(_originalForwardType,0f));
    }

    public override void OnUnitReset()
    {
        base.OnUnitReset();
        _sequence?.Kill();
        StartCoroutine(WaitForRotate(_originalForwardType,0f));
    }

    /// <summary>
    /// player移动指令
    /// </summary>
    /// <param name="commandEnum">指令类型</param>
    /// <param name="during">移动时间</param>
    /// <returns></returns>
    public IEnumerator MoveCommand(CommandType commandEnum,float during=1f)
    {
        Vector2Int lastPoint=GridManager.Instance.GetPointByWorldPosition(transform.position);
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
        
        if (_currentForwardType!=targetForward)
        {
            yield return StartCoroutine(WaitForRotate(targetForward,0.3f));
            during -= 0.3f;
        }
        //Debug.Log(GridManager.Instance.CheckWalkable(targetPoint.x, targetPoint.y));
        // Debug.Log(newPoint.x+"  "+newPoint.y);
        if (BattleManager.Instance.CheckWalkable(newPoint.x, newPoint.y))
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOMove(GridManager.Instance.GetWorldPositionByPoint(newPoint.x, newPoint.y), during));
            _sequence.SetAutoKill(false);
            //Debug.Log(GridManager.Instance.GetWorldPositionByPoint(targetPoint.x, targetPoint.y));
            yield return _sequence.WaitForCompletion();
        }
        else
        {
            yield return new WaitForSeconds(during);
        }
        
        BattleManager.Instance.UpdateUnitPoint(this);//更新GridObj

        if (CommandManager.Instance.RefreshCacheCurrentTimeTextInExecuting())//如果超时了
        {
            yield break;
        }
        
        if (!BattleManager.Instance.CheckPlayerGetTarget())
        {
            CommandManager.Instance.ExecuteCommand();
        }
    }
    
    /// <summary>
    /// 旋转到目标方向
    /// </summary>
    /// <param name="targetForward">目标方向</param>
    /// <param name="during">旋转时间</param>
    /// <returns></returns>
    private IEnumerator WaitForRotate(ForwardType targetForward,float during=0.3f)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        switch (targetForward)
        {
            case ForwardType.Up:
                _sequence.Append(transform.DORotate(new Vector3(0,0,0),during)) ;
                break;
            case ForwardType.Down:
                _sequence.Append(transform.DORotate(new Vector3(0,180,0),during));
                break;
            case ForwardType.Left:
                _sequence.Append(transform.DORotate(new Vector3(0,-90,0),during));
                break;
            case ForwardType.Right:
                _sequence.Append(transform.DORotate(new Vector3(0,90,0),during));
                break;
        }

        yield return _sequence.WaitForCompletion();
        _currentForwardType = targetForward;
    } 
    
}
