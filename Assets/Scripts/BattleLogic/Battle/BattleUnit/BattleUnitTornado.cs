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

    private float _perMoveCellTime;
    private Sequence _sequence;
    private EventManager.TypeEvent _onCommandExecuteStartAction;
    private EventManager.TypeEvent _onCommandExecuteAction;
    private int _cacheDelayTime;
    private ForwardType _currentForward;
    private bool _hasStartMove; //包括delay
    private bool _isPlayingTween;

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        _perMoveCellTime = Mathf.Round(1f / speed);
        SetAction();
        EventManager.Instance.AddListener(EventName.OnCommandExecuteStart, _onCommandExecuteStartAction);
        EventManager.Instance.AddListener(EventName.OnCommandExecute, _onCommandExecuteAction);
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
        EventManager.Instance.RemoveListener(EventName.OnCommandExecuteStart, _onCommandExecuteStartAction);
        EventManager.Instance.RemoveListener(EventName.OnCommandExecute, _onCommandExecuteAction);
    }

    private void SetAction()
    {
        _onCommandExecuteStartAction += OnCommandExecuteStart;
        _onCommandExecuteAction += ActionEveryExecute;
    }

    private void ResetAll()
    {
        _isPlayingTween = false;
        _cacheDelayTime = delayTime;
        _currentForward = originalForwardType;
        ResetTweener();
    }

    private void ResetTweener()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(CalculateNextTargetPosition(), _perMoveCellTime));
        _sequence.Pause();
        _sequence.SetAutoKill(false);
        _sequence.onComplete += OnMoveComplete;
    }

    private void OnMoveComplete()
    {
        _isPlayingTween = false;
        BattleManager.Instance.UpdateUnitPoint(this);
        CheckCurrentPoint();
        ResetTweener();
    }

    private void OnCommandExecuteStart()
    {
        if (moveOnStart)
        {
            _hasStartMove = true;
        }
    }

    private void ActionEveryExecute()
    {
        if (!_hasStartMove)
        {
            return;
        }

        if (_cacheDelayTime > 0)
        {
            _cacheDelayTime--;
            return;
        }

        if (_isPlayingTween)
        {
            return;
        }

        _sequence.Play();
        _isPlayingTween = true;
    }

    private Vector3 CalculateNextTargetPosition()
    {
        Vector3 targetPosition = transform.position;
        float cellSize = GridManager.Instance.GetPerCellSize();
        switch (_currentForward)
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

        Vector2Int targetPoint = GridManager.Instance.GetPointByWorldPosition(targetPosition);
        //禁止掉头两次还是walkable，即不能被困住
        if (!BattleManager.Instance.CheckWalkable(targetPoint.x, targetPoint.y))
        {
            return TurnRound();
        }

        //Debug.Log(transform.position+"--->"+targetPosition);
        return targetPosition;
    }

    private Vector3 TurnRound()
    {
        switch (_currentForward)
        {
            case ForwardType.Up:
                _currentForward = ForwardType.Down;
                break;
            case ForwardType.Down:
                _currentForward = ForwardType.Up;
                break;
            case ForwardType.Left:
                _currentForward = ForwardType.Right;
                break;
            case ForwardType.Right:
                _currentForward = ForwardType.Left;
                break;
        }

        return CalculateNextTargetPosition();
    }

    private void CheckCurrentPoint()
    {
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
                people[i].SetPeopleActive(false);
            }
        });
    }
}