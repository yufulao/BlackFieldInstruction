using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleUnitCarRear : BattleUnit
{
    [SerializeField] private BattleUnitCarFront carFront;
    [SerializeField] private ForwardType originalForwardType;
    [SerializeField] private GameObject carObj;

    private BattleUnitPlayer _player;
    private Sequence _sequence;
    private Vector3 _cacheCraObjOriginalPosition;
    private ForwardType _currentForward;
    private CommandType _cacheCommandType;

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        _cacheCraObjOriginalPosition = carObj.transform.position;
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
        _cacheCommandType = commandType;
    }

    public override IEnumerator Execute()
    {
        yield return base.Execute();
        yield return ExecuteEveryCommand();
    }

    public override IEnumerator CheckOverlap()
    {
        yield return base.CheckOverlap();
        if (_player!=null)
        {
            CheckUnit();
        }
    }

    /// <summary>
    /// 每次执行指令时
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExecuteEveryCommand()
    {
        if (_player == null)
        {
            yield break;
        }
        if (_cacheCommandType==CommandType.Wait)
        {
            yield return new WaitForSeconds(1f);
            yield break;
        }
            
        ForwardType commandForward=ForwardType.Up;
        switch (_cacheCommandType)
        {
            case CommandType.Up:
                commandForward = ForwardType.Up;
                break;
            case CommandType.Down:
                commandForward = ForwardType.Down;
                break;
            case CommandType.Right:
                commandForward = ForwardType.Right;
                break;
            case CommandType.Left:
                commandForward = ForwardType.Left;
                break;
        }

        if (_currentForward!=commandForward)
        {
            yield return TurnAroundOrGetOff(commandForward);
            yield break;
        }
            
        yield return Drive();
    }

    /// <summary>
    /// 上车，player判断unit是不是CarRear
    /// </summary>
    /// <param name="player"></param>
    public void GetOn(BattleUnitPlayer player)
    {
        _player = player;
        _player.car = this;
        _player.gameObject.transform.SetParent(transform);
        _player.RefreshPlayerObjActive(false);
    }

    /// <summary>
    /// 执行调头还是下车
    /// </summary>
    /// <param name="commandForward"></param>
    /// <returns></returns>
    private IEnumerator TurnAroundOrGetOff(ForwardType commandForward)
    {
        //_currentForward != targetForward;
        bool isTurnAround = false;
        switch (_currentForward)
        {
            case ForwardType.Up:
                if (commandForward == ForwardType.Down)
                {
                    isTurnAround = true;
                }

                break;
            case ForwardType.Down:
                if (commandForward == ForwardType.Up)
                {
                    isTurnAround = true;
                }

                break;
            case ForwardType.Left:
                if (commandForward == ForwardType.Right)
                {
                    isTurnAround = true;
                }

                break;
            case ForwardType.Right:
                if (commandForward == ForwardType.Left)
                {
                    isTurnAround = true;
                }

                break;
        }

        if (isTurnAround)
        {
            yield return TurnAround(commandForward);
            yield break;
        }

        yield return GetOff(commandForward);
    }

    /// <summary>
    /// 执行行驶
    /// </summary>
    /// <returns></returns>
    private IEnumerator Drive()
    {
        Vector2Int rearTargetPoint = Vector2Int.zero;
        Vector2Int frontTargetPoint = Vector2Int.zero;
        if (!IfCanDrive((rearTarget, frontTarget) =>
        {
            rearTargetPoint = rearTarget;
            frontTargetPoint = frontTarget;
        }))
        {
            yield return DriveFail();
            yield break;
        }

        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Join(transform.DOMove(GridManager.Instance.GetWorldPositionByPoint(rearTargetPoint.x, rearTargetPoint.y), 1f));
        // Debug.Log(transform.position);
        // Debug.Log(rearTargetPoint);
        _sequence.Join(carFront.gameObject.transform.DOMove(GridManager.Instance.GetWorldPositionByPoint(frontTargetPoint.x, frontTargetPoint.y), 1f));
        _sequence.SetAutoKill(false);
        BattleManager.Instance.UpdateUnitPoint(this, rearTargetPoint);
        BattleManager.Instance.UpdateUnitPoint(carFront, frontTargetPoint);
        BattleManager.Instance.UpdateUnitPoint(_player, rearTargetPoint);

        yield return _sequence.WaitForCompletion();
    }

    /// <summary>
    /// 执行调头
    /// </summary>
    /// <param name="targetForwardType"></param>
    /// <returns></returns>
    private IEnumerator TurnAround(ForwardType targetForwardType)
    {
        Vector2Int frontTargetPoint = Vector2Int.zero;
        if (!IfCanTurnRound(targetForwardType, (targetPoint) => { frontTargetPoint = targetPoint; }))
        {
            yield return TurnAroundFail();
            yield break;
        }

        carFront.transform.position = GridManager.Instance.GetWorldPositionByPoint(frontTargetPoint.x, frontTargetPoint.y);
        BattleManager.Instance.UpdateUnitPoint(carFront, frontTargetPoint);
        CheckUnit();
        _currentForward = targetForwardType;
        RefreshCarObj();
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// 执行下车
    /// </summary>
    /// <param name="targetForward"></param>
    /// <returns></returns>
    private IEnumerator GetOff(ForwardType targetForward)
    {
        Vector2Int getOffPoint = GridManager.Instance.GetPointByWorldPosition(carFront.transform.position);
        switch (_currentForward)
        {
            case ForwardType.Up:
            case ForwardType.Down:
                if (targetForward == ForwardType.Right)
                {
                    getOffPoint += new Vector2Int(1, 0);
                }

                if (targetForward == ForwardType.Left)
                {
                    getOffPoint += new Vector2Int(-1, 0);
                }

                break;
            case ForwardType.Left:
            case ForwardType.Right:
                if (targetForward == ForwardType.Up)
                {
                    getOffPoint += new Vector2Int(0, 1);
                }

                if (targetForward == ForwardType.Down)
                {
                    getOffPoint += new Vector2Int(0, -1);
                }

                break;
        }

        //判断能不能下车
        if (BattleManager.Instance.CheckWalkable(getOffPoint))
        {
            _player.RefreshPlayerObjActive(true);
            _player.transform.position = GridManager.Instance.GetWorldPositionByPoint(getOffPoint.x, getOffPoint.y);
            BattleManager.Instance.UpdateUnitPoint(_player, getOffPoint);
            _player.car = null;
            _player = null;
            yield return new WaitForSeconds(1f);
            yield break;
        }

        yield return GetOffFail();
    }

    /// <summary>
    /// 检测特定unit
    /// </summary>
    private void CheckUnit()
    {
        BattleManager.Instance.CheckCellForUnit<BattleUnitPeople>(this, UnitType.People, (people) =>
        {
            for (int i = 0; i < people.Count; i++)
            {
                people[i].SetPeopleActive(false);
            }
        });
        BattleManager.Instance.CheckCellForUnit<BattleUnitPeople>(carFront, UnitType.People, (people) =>
        {
            for (int i = 0; i < people.Count; i++)
            {
                people[i].SetPeopleActive(false);
            }
        });
    }

    /// <summary>
    /// 检测车是行驶两格还是一格还是不能行驶，返回可不可以行驶
    /// </summary>
    /// <returns></returns>
    private bool IfCanDrive(Action<Vector2Int,Vector2Int> callback)
    {
        //先判断一格再判断两格,然后才DriveFail
        Vector2Int point1 = GridManager.Instance.GetPointByWorldPosition(carFront.gameObject.transform.position);
        Vector2Int point2 = point1;
        switch (_currentForward)
        {
            case ForwardType.Up:
                point1 += new Vector2Int(0, 1);
                point2 += new Vector2Int(0, 2);
                break;
            case ForwardType.Down:
                point1 += new Vector2Int(0, -1);
                point2 += new Vector2Int(0, -2);
                break;
            case ForwardType.Left:
                point1 += new Vector2Int(-1, 0);
                point2 += new Vector2Int(-2, 0);
                break;
            case ForwardType.Right:
                point1 += new Vector2Int(1, 0);
                point2 += new Vector2Int(2, 0);
                break;
        }

        // Debug.Log(GridManager.Instance.GetPointByWorldPosition(carFront.gameObject.transform.position));
        // Debug.Log(point1);
        // Debug.Log(point2);

        //一格都走不了
        if (!BattleManager.Instance.CheckWalkable(point1))
        {
            return false;
        }
        //第一格可以走
        
        //尝试走两格
        if (BattleManager.Instance.CheckWalkable(point2))
        {
            callback?.Invoke(point1,point2);
            return true;
        }

        //只走一格
        callback?.Invoke(GridManager.Instance.GetPointByWorldPosition(carFront.gameObject.transform.position),point1);
        return true;
    }

    /// <summary>
    /// 检测是否可以调头
    /// </summary>
    /// <param name="targetForwardType"></param>
    /// <param name="callback">front的targetPoint</param>
    /// <returns></returns>
    private bool IfCanTurnRound(ForwardType targetForwardType, Action<Vector2Int> callback)
    {
        Vector2Int carFrontTargetPoint = GridManager.Instance.GetPointByWorldPosition(transform.position);
        switch (targetForwardType)
        {
            case ForwardType.Up:
                carFrontTargetPoint += new Vector2Int(0, 1);
                break;
            case ForwardType.Down:
                carFrontTargetPoint += new Vector2Int(0, -1);
                break;
            case ForwardType.Left:
                carFrontTargetPoint += new Vector2Int(-1, 0);
                break;
            case ForwardType.Right:
                carFrontTargetPoint += new Vector2Int(1, 0);
                break;
        }

        callback?.Invoke(carFrontTargetPoint);
        return BattleManager.Instance.CheckWalkable(carFrontTargetPoint);
    }

    /// <summary>
    /// 调头失败
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnAroundFail()
    {
        transform.DOShakePosition(0.2f, 0.1f, 1, 0.1f, true);
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// 行驶失败
    /// </summary>
    /// <returns></returns>
    private IEnumerator DriveFail()
    {
        transform.DOShakePosition(0.5f);
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// 下车失败
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetOffFail()
    {
        transform.DOShakePosition(0.5f);
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// 刷新车obj的朝向和位置
    /// </summary>
    private void RefreshCarObj()
    {
        switch (_currentForward)
        {
            case ForwardType.Up:
                carObj.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                break;
            case ForwardType.Down:
                carObj.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                break;
            case ForwardType.Left:
                carObj.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
                break;
            case ForwardType.Right:
                carObj.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                break;
        }

        carObj.transform.position = (transform.position + carFront.gameObject.transform.position) / 2f;
        // Debug.Log(transform.position);
        // Debug.Log(carFront.gameObject.transform.position);
        // Debug.Log(((transform.position + carFront.gameObject.transform.position) / 2f));
        // Debug.Log(carObj.transform.position);
        //Debug.Log(_currentForward);
    }

    /// <summary>
    /// 重置unit
    /// </summary>
    private void ResetAll()
    {
        //车头车尾 已归位
        _sequence?.Kill();
        _player = null;
        _currentForward = originalForwardType;
        RefreshCarObj();
        carObj.transform.position = _cacheCraObjOriginalPosition;
    }
}