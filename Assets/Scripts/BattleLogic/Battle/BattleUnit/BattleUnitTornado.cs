using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Sequence = DG.Tweening.Sequence;

public class BattleUnitTornado : BattleUnit
{
    [SerializeField] private float speed;
    [SerializeField] private ForwardType originalForwardType;
    [SerializeField] private bool moveOnStart;
    [SerializeField] private int delayTime;
    
    private Sequence _sequence;
    private EventManager.TypeEvent _onCommandExecuteStartAction;
    private EventManager.TypeEvent _onCommandExecuteAction;
    private int _cacheDelayTime;
    [HideInInspector]public ForwardType currentForward;
    private bool _hasStartMove; //包括delay
    [HideInInspector]public Vector3 cacheLastPosition;

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        EventManager.Instance.AddListener(EventName.CommandMainStart, OnCommandMainStart);
        EventManager.Instance.AddListener(EventName.CommandMainEnd,OnCommandMainEnd);
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
        EventManager.Instance.RemoveListener(EventName.CommandMainStart, OnCommandMainStart);
        EventManager.Instance.RemoveListener(EventName.CommandMainEnd,OnCommandMainEnd);
    }

    public override IEnumerator Execute()
    {
        yield return base.Execute();
        yield return ActionEveryExecute();
    }

    public override IEnumerator CheckOverlap()
    {
        yield return base.CheckOverlap();
        CheckUnit();
    }

    public override IEnumerator Calculate(CommandType commandType)
    {
        yield return base.Calculate(commandType);
        Vector3 targetPosition = CalculateNextTargetPosition();
        ResetTweener(targetPosition);
    }

    /// <summary>
    /// 重置unit
    /// </summary>
    private void ResetAll()
    {
        _sequence?.Kill();
        _cacheDelayTime = delayTime;
        currentForward = originalForwardType;
        SfxManager.Instance.Stop("unit_tornadoMove");
    }

    /// <summary>
    /// 重置tweener
    /// </summary>
    private void ResetTweener(Vector3 targetPosition)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(targetPosition, 1f));
        _sequence.Pause();
        _sequence.SetAutoKill(false);
    }

    /// <summary>
    /// 当第一条指令开始执行时，应用是否一开始就可以自动移动
    /// </summary>
    private void OnCommandMainStart()
    {
        if (moveOnStart)
        {
            SfxManager.Instance.PlaySfx("unit_tornadoMove");
            _hasStartMove = true;
        }
    }

    /// <summary>
    /// 当所有指令都执行结束
    /// </summary>
    private void OnCommandMainEnd()
    {
        SfxManager.Instance.Stop("unit_tornadoMove");
    }

    /// <summary>
    /// 每次执行指令时
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActionEveryExecute()
    {
        cacheLastPosition = transform.position;
        
        if (!_hasStartMove)
        {
            yield break;
        }

        if (_cacheDelayTime > 0)
        {
            _cacheDelayTime--;
            yield break;
        }
        
        _sequence.Play();
        yield return _sequence.WaitForCompletion();
        BattleManager.Instance.UpdateUnitPoint(this);
    }

    /// <summary>
    /// 计算下一个指令执行时的目标坐标点
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateNextTargetPosition()
    {
        //如果自己的地块就不能走，就停在原地
        if (!BattleManager.Instance.CheckWalkable(transform.position))
        {
            return transform.position;
        }
        
        Vector3 targetPosition = GetForwardTargetPosition();
        //如果可以走
        if (BattleManager.Instance.CheckWalkable(targetPosition))
        {
            return targetPosition;
        }
        
        //不能走，先调头
        TurnRound();
        targetPosition = GetForwardTargetPosition();
        //调头后如果可以走
        if (BattleManager.Instance.CheckWalkable(targetPosition))
        {
            return targetPosition;
        }
        
        //调了头也不能走
        return transform.position;

        //Debug.Log(transform.position+"--->"+targetPosition);
    }

    /// <summary>
    /// 调头前先设置改变当前朝向，再应用当前朝向的动画
    /// </summary>
    /// <returns></returns>
    private void TurnRound()
    {
        switch (currentForward)
        {
            case ForwardType.Up:
                currentForward = ForwardType.Down;
                break;
            case ForwardType.Down:
                currentForward = ForwardType.Up;
                break;
            case ForwardType.Left:
                currentForward = ForwardType.Right;
                break;
            case ForwardType.Right:
                currentForward = ForwardType.Left;
                break;
        }
    }

    private Vector3 GetForwardTargetPosition()
    {
        Vector3 targetPosition = transform.position;
        float cellSize = GridManager.Instance.GetPerCellSize();
        switch (currentForward)
        {
            case ForwardType.Up:
                targetPosition += new Vector3(0, 0, cellSize);
                break;
            case ForwardType.Down:
                targetPosition += new Vector3(0, 0, -cellSize);
                break;
            case ForwardType.Left:
                targetPosition += new Vector3(-cellSize, 0, 0);
                break;
            case ForwardType.Right:
                targetPosition += new Vector3(cellSize, 0, 0);
                break;
        }
        return targetPosition;
    }

    /// <summary>
    /// 检测特定unit
    /// </summary>
    private void CheckUnit()
    {
        if (BattleManager.Instance.CheckCellForUnit<BattleUnitCarFront>(this, UnitType.CarFront) || BattleManager.Instance.CheckCellForUnit<BattleUnitCarRear>(this, UnitType.CarRear))
        {
            BackToLastPosition();
            return;
        }

        bool needBack = false;
        BattleManager.Instance.CheckCellForOrderPoint<BattleUnitRuin>(GridManager.Instance.GetPointByWorldPosition(cacheLastPosition), UnitType.Ruin, (ruins) =>
        {
            for (int i = 0; i < ruins.Count; i++)
            {
                if (ruins[i].currentActive)
                {
                    needBack = true;
                    BackToLastPosition();
                }
            }
        });
        if (needBack)
        {
            return;
        }
        
        BattleManager.Instance.CheckCellForUnit<BattleUnitFire>(this, UnitType.Fire, (fireUnits) =>
        {
            for (int i = 0; i < fireUnits.Count; i++)
            {
                fireUnits[i].transform.GetComponent<BattleUnitFire>().SetFireActive(false);
            }
        });

        BattleManager.Instance.CheckCellForUnit<BattleUnitPeople>(this, UnitType.People, (people) =>
        {
            for (int i = 0; i < people.Count; i++)
            {
                people[i].PeopleDie();
            }
        });
    }

    /// <summary>
    /// 返回上一个点
    /// </summary>
    private void BackToLastPosition()
    {
        transform.position = cacheLastPosition;
        BattleManager.Instance.UpdateUnitPoint(this);
    }

    
}