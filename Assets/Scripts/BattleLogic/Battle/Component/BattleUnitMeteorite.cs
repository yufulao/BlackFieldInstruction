using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleUnitMeteorite : BattleUnit
{
    [SerializeField] private bool fallOnStart;
    [SerializeField] private int delayTime;
    [SerializeField] private int fallTime;
    [SerializeField] private GameObject meteorite;
    [SerializeField] private Transform targetFallPosition;
    [SerializeField] private BattleUnitFire _unitFire;

    private Sequence _sequence;
    private bool _hasStartFall;
    private bool _hadPlayTween;
    private EventManager.TypeEvent _onCommandExecuteStartAction;
    private EventManager.TypeEvent _onCommandExecuteAction;
    private int _cacheDelayTime;
    private Vector3 _meteoriteOriginalPosition;


    public override void OnUnitInit()
    {
        base.OnUnitInit();
        _meteoriteOriginalPosition = meteorite.transform.position;
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

    private void OnCommandExecuteStart()
    {
        if (fallOnStart)
        {
            _hasStartFall = true;
        }
    }

    private void ResetAll()
    {
        _hasStartFall = false;
        _hadPlayTween = false;
        _cacheDelayTime = delayTime;
        _unitFire.SetFireActive(false);
        ResetTweener();
    }

    private void ResetTweener()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        meteorite.SetActive(true);
        meteorite.transform.position = _meteoriteOriginalPosition;
        _sequence.Append(meteorite.transform.DOMove(targetFallPosition.position, fallTime));
        _sequence.Pause();
        _sequence.SetAutoKill(false);
        _sequence.onComplete += OnMeteoriteFallDone;
    }

    private void ActionEveryExecute()
    {
        if (!_hasStartFall)
        {
            return;
        }
        
        if (_cacheDelayTime > 0)
        {
            _cacheDelayTime--;
            return;
        }

        if (_hadPlayTween)
        {
            return;
        }
        
        _sequence.Play();
        _hadPlayTween = true;
    }

    /// <summary>
    /// 播放完毕时，也就是陨石落地时
    /// </summary>
    private void OnMeteoriteFallDone()
    {
        meteorite.SetActive(false);
        _unitFire.SetFireActive(true);
    }
}