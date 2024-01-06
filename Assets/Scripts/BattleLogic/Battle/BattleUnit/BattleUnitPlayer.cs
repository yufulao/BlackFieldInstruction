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
    private Transform _cachePlayerParent;
    private Vector2Int _cacheTargetPoint;
    private ForwardType _cacheTargetForward;
    private int _currentPeopleCount;
    private bool _cacheIsGetOnCommand;

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

    public override void OnUnitDestroy()
    {
        base.OnUnitDestroy();
        _sequence?.Kill();
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
        if (car)
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
        _cacheIsGetOnCommand = false;
        animator.SetBool("run", false);
        SfxManager.Instance.Stop("unit_playerMove");
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
            if (_cacheIsGetOnCommand)
            {
                car.GetOn(this);
                _cacheIsGetOnCommand = false;
            }

            yield break;
        }

        if (BattleManager.Instance.CheckWalkable(targetPoint.x, targetPoint.y))
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOMove(GridManager.Instance.GetWorldPositionByPoint(targetPoint.x, targetPoint.y), during));
            if (GridManager.Instance.GetPointByWorldPosition(transform.position) != targetPoint)
            {
                SfxManager.Instance.PlaySfx("unit_playerMove");
                animator.SetBool("run", true);
            }

            _sequence.SetAutoKill(false);
            BattleManager.Instance.UpdateUnitPoint(this, targetPoint); //更新GridObj
            //Debug.Log(GridManager.Instance.GetWorldPositionByPoint(targetPoint.x, targetPoint.y));
            yield return _sequence.WaitForCompletion();
            SfxManager.Instance.Stop("unit_playerMove");
            animator.SetBool("run", false);
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
            SfxManager.Instance.PlaySfx("unit_carGetOn");
            car = rear[0];
            gameObject.transform.SetParent(transform);
            RefreshPlayerObjActive(false);
            _cacheIsGetOnCommand = true;
            canWalk = false;
        });

        return canWalk;
    }

    /// <summary>
    /// player移动后的检测
    /// </summary>
    private void CheckAfterPlayerMove()
    {
        if (car)
        {
            return;
        }

        BattleManager.Instance.CheckCellForUnit<BattleUnitTarget>(this, UnitType.Target, (targets) =>
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (_currentPeopleCount >= targets[i].needPeopleCount)
                {
                    if (CommandManager.Instance.CheckMoreThanTime()) //更新左上角时间，如果超时
                    {
                        BattleManager.Instance.BattleEnd(false);
                        return;
                    }

                    BattleManager.Instance.BattleEnd(true);
                    return;
                }
            }
        });

        BattleManager.Instance.CheckCellForUnit<BattleUnitFire>(this, UnitType.Fire, (fires) =>
        {
            for (int i = 0; i < fires.Count; i++)
            {
                if (fires[i].currentActive)
                {
                    BattleManager.Instance.BattleEnd(false);
                    return;
                }
            }
        });

        BattleManager.Instance.CheckCellForUnit<BattleUnitTornado>(this, UnitType.Tornado, (tornadoes) =>
        {
            if (tornadoes.Count > 0)
            {
                BattleManager.Instance.BattleEnd(false);
            }
        });

        BattleManager.Instance.CheckCellForUnit<BattleUnitRuin>(this, UnitType.Ruin, (ruins) =>
        {
            for (int i = 0; i < ruins.Count; i++)
            {
                if (ruins[i].currentActive)
                {
                    BattleManager.Instance.BattleEnd(false);
                    return;
                }
            }
        });

        BattleManager.Instance.CheckCellForUnit<BattleUnitPeople>(this, UnitType.People, (people) =>
        {
            for (int i = 0; i < people.Count; i++)
            {
                people[i].PeopleHelp();
                _currentPeopleCount++;
                BattleManager.Instance.SetTargetState(_currentPeopleCount);
            }
        });
        
        List<BattleUnit> allUnit = BattleManager.Instance.GetAllUnit();
        for (int i = 0; i < allUnit.Count; i++)
        {
            if (allUnit[i].unitType == UnitType.Tornado)
            {
                BattleUnitTornado tornado = allUnit[i].transform.GetComponent<BattleUnitTornado>();
                if (GridManager.Instance.GetPointByWorldPosition(transform.position)==GridManager.Instance.GetPointByWorldPosition(tornado.cacheLastPosition))//如果当前站在tornado的上一个坐标点
                {
                    if (CheckTornadoForwardMove(tornado.currentForward))//如果正面相撞
                    {
                        BattleManager.Instance.BattleEnd(false);
                        break;
                    }
                }
            }
        }
        
        
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

    /// <summary>
    /// 检测player和tornado面对面直撞
    /// </summary>
    /// <returns></returns>
    private bool CheckTornadoForwardMove(ForwardType tornadoForward)
    {
        bool isHit = false;
        switch (_currentForwardType)
        {
            case ForwardType.Up:
                if (tornadoForward == ForwardType.Down)
                {
                    isHit = true;
                }

                break;
            case ForwardType.Down:
                if (tornadoForward == ForwardType.Up)
                {
                    isHit = true;
                }

                break;
            case ForwardType.Left:
                if (tornadoForward == ForwardType.Right)
                {
                    isHit = true;
                }

                break;
            case ForwardType.Right:
                if (tornadoForward == ForwardType.Left)
                {
                    isHit = true;
                }

                break;
        }

        return isHit;
    }
}