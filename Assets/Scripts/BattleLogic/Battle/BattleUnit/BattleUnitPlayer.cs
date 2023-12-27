using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleUnitPlayer : BattleUnit
{
    [SerializeField] private GameObject playerObj;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private ForwardType originalForwardType = ForwardType.Up;
    private ForwardType _currentForwardType;
    private Sequence _sequence;
    [HideInInspector] public BattleUnitCarRear car;
    [HideInInspector] private Transform _cachePlayerParent;
    private Vector2Int _cacheTargetPoint;
    private ForwardType _cacheTargetForward;
    private int _currentPeopleCount;

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        _cachePlayerParent = transform.parent;
        ResetAll();
    }

    public override void OnUnitReset()
    {
        base.OnUnitReset();
        ResetAll();
    }

    public override IEnumerator Calculate(CommandType commandType)
    {
        yield return base.Calculate(commandType);
        CalculateTarget(commandType);
    }

    public override IEnumerator Execute()
    {
        yield return base.Execute();
        yield return ExecuteEveryCommand();
    }

    public override IEnumerator CheckOverlap()
    {
        yield return base.CheckOverlap();
        CheckAfterPlayerMove();
    }

    /// <summary>
    /// 计算当前指令的目标坐标点
    /// </summary>
    /// <param name="commandEnum">指令类型</param>
    /// <returns></returns>
    private void CalculateTarget(CommandType commandEnum)
    {
        Vector2Int lastPoint = GridManager.Instance.GetPointByWorldPosition(transform.position);
        ForwardType targetForward = _currentForwardType;
        Vector2Int targetPoint = lastPoint;
        switch (commandEnum)
        {
            case CommandType.Up:
                targetForward = ForwardType.Up;
                targetPoint = new Vector2Int(lastPoint.x, lastPoint.y + 1);
                break;
            case CommandType.Down:
                targetForward = ForwardType.Down;
                targetPoint = new Vector2Int(lastPoint.x, lastPoint.y - 1);
                break;
            case CommandType.Left:
                targetForward = ForwardType.Left;
                targetPoint = new Vector2Int(lastPoint.x - 1, lastPoint.y);
                break;
            case CommandType.Right:
                targetForward = ForwardType.Right;
                targetPoint = new Vector2Int(lastPoint.x + 1, lastPoint.y);
                break;
            case CommandType.Wait:
                targetPoint = new Vector2Int(lastPoint.x, lastPoint.y);
                break;
        }

        if (car != null)
        {
            //在车内时转存车的指令
            return;
        }

        _cacheTargetForward = targetForward;
        _cacheTargetPoint = targetPoint;
    }

    /// <summary>
    /// 每次执行指令时
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExecuteEveryCommand()
    {
        if (car != null)
        {
            //在车内时转存车的指令
            yield break;
        }
        
        float during = 1f;
        if (_currentForwardType != _cacheTargetForward)
        {
            yield return StartCoroutine(WaitForRotate(_cacheTargetForward, 0.3f));
            during -= 0.3f;
        }
        yield return Move(_cacheTargetPoint, during);
    }

    /// <summary>
    /// 刷新player的可见状态
    /// </summary>
    /// <param name="active"></param>
    public void RefreshPlayerObjActive(bool active)
    {
        playerObj.SetActive(active);
        if (active)
        {
            transform.SetParent(_cachePlayerParent);
        }
    }

    /// <summary>
    /// 重置unit
    /// </summary>
    private void ResetAll()
    {
        RefreshPlayerObjActive(true);
        _sequence?.Kill();
        car = null;
        _currentPeopleCount = 0;
        StartCoroutine(WaitForRotate(originalForwardType, 0f));
    }

    /// <summary>
    /// 执行移动
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <param name="during"></param>
    /// <returns></returns>
    private IEnumerator Move(Vector2Int targetPoint, float during)
    {
        if (!CheckBeforePlayerMove(targetPoint)) //移动前检测
        {
            yield return new WaitForSeconds(during);
            yield break;
        }

        if (BattleManager.Instance.CheckWalkable(targetPoint.x, targetPoint.y))
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOMove(GridManager.Instance.GetWorldPositionByPoint(targetPoint.x, targetPoint.y), during));
            _sequence.SetAutoKill(false);
            BattleManager.Instance.UpdateUnitPoint(this,targetPoint); //更新GridObj
            //Debug.Log(GridManager.Instance.GetWorldPositionByPoint(targetPoint.x, targetPoint.y));
            yield return _sequence.WaitForCompletion();
        }
        else
        {
            yield return new WaitForSeconds(during);
        }
    }

    /// <summary>
    /// player移动前的检测
    /// </summary>
    /// <returns></returns>
    private bool CheckBeforePlayerMove(Vector2Int targetPoint)
    {
        bool canWalk = true;
        BattleManager.Instance.CheckCellForOrderPoint<BattleUnitBrokenBuilding>(targetPoint, UnitType.BrokenBuilding, (brokenBuildings) =>
        {
            for (int i = 0; i < brokenBuildings.Count; i++)
            {
                brokenBuildings[i].BrokenBuilding(_currentForwardType);
                if (!brokenBuildings[i].GetWalkable())
                {
                    canWalk = false;
                }
            }
        });

        BattleManager.Instance.CheckCellForOrderPoint<BattleUnitCarRear>(targetPoint, UnitType.CarRear, (rear) =>
        {
            car = rear[0];
            rear[0].GetOn(this);
        });

        return canWalk;
    }

    /// <summary>
    /// player移动后的检测
    /// </summary>
    private void CheckAfterPlayerMove()
    {
        BattleManager.Instance.CheckCellForUnit<BattleUnitTarget>(this, UnitType.Target, (targets) =>
        {
            if (car==null)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (_currentPeopleCount>=targets[i].needPeopleCount)
                    {
                        CommandManager.Instance.ForceChangeToMainEnd();
                        BattleManager.Instance.BattleEnd(true);
                        return;
                    }
                }
            }
        });

        BattleManager.Instance.CheckCellForUnit<BattleUnitFire>(this, UnitType.Fire, (fires) =>
        {
            if (car==null)
            {
                for (int i = 0; i < fires.Count; i++)
                {
                    if (fires[i].currentActive)
                    {
                        CommandManager.Instance.ForceChangeToMainEnd();
                        BattleManager.Instance.BattleEnd(false);
                        return;
                    }
                }
            }
        });

        BattleManager.Instance.CheckCellForUnit<BattleUnitPeople>(this, UnitType.People, (people) =>
        {
            if (car==null)
            {
                for (int i = 0; i < people.Count; i++)
                {
                    people[i].SetPeopleActive(false);
                    _currentPeopleCount++;
                }
            }
        });
    }

    /// <summary>
    /// 旋转到目标方向
    /// </summary>
    /// <param name="targetForward">目标方向</param>
    /// <param name="during">旋转时间</param>
    /// <returns></returns>
    private IEnumerator WaitForRotate(ForwardType targetForward, float during = 0.3f)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        switch (targetForward)
        {
            case ForwardType.Up:
                _sequence.Append(transform.DORotate(new Vector3(0, 0, 0), during));
                break;
            case ForwardType.Down:
                _sequence.Append(transform.DORotate(new Vector3(0, 180, 0), during));
                break;
            case ForwardType.Left:
                _sequence.Append(transform.DORotate(new Vector3(0, -90, 0), during));
                break;
            case ForwardType.Right:
                _sequence.Append(transform.DORotate(new Vector3(0, 90, 0), during));
                break;
        }

        yield return _sequence.WaitForCompletion();
        _currentForwardType = targetForward;
    }
}