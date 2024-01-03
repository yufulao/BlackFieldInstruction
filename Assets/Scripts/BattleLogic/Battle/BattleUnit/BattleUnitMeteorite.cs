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
    [SerializeField] private BattleUnitFire unitFire;
    [SerializeField] private ParticleSystem boomVfx;
    private Vector3 _meteoriteOriginalPosition;

    private Sequence _sequence;
    private bool _hasStartFall;
    private int _cacheDelayTime;
    private int _cacheFallTime;


    public override void OnUnitInit()
    {
        base.OnUnitInit();
        EventManager.Instance.AddListener(EventName.CommandMainStart,OnCommandMainStart);
        _meteoriteOriginalPosition = meteorite.transform.position;
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
        EventManager.Instance.RemoveListener(EventName.CommandMainStart,OnCommandMainStart);
    }

    public override IEnumerator Execute()
    {
        yield return base.Execute();
        yield return ActionEveryExecute();
    }

    /// <summary>
    /// 在指令一开始时应用，是否一开始就下落陨石
    /// </summary>
    private void OnCommandMainStart()
    {
        if (fallOnStart)
        {
            _hasStartFall = true;
        }
    }

    /// <summary>
    /// 重置unit
    /// </summary>
    private void ResetAll()
    {
        _hasStartFall = false;
        _cacheDelayTime = delayTime;
        _cacheFallTime = fallTime;
        meteorite.SetActive(true);
        //unitFire.SetFireActive(true);//fire单位自己有reset
        boomVfx.Stop();
        ResetTweener();
    }

    /// <summary>
    /// 重新设置tweener
    /// </summary>
    private void ResetTweener()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        meteorite.transform.position = _meteoriteOriginalPosition;
        _sequence.Append(meteorite.transform.DOMove(targetFallPosition.position, fallTime));
        _sequence.Pause();
        _sequence.SetAutoKill(false);
    }

    /// <summary>
    /// 每次执行指令时
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActionEveryExecute()
    {
        if (!_hasStartFall)
        {
            yield break;
        }
        
        if (_cacheDelayTime > 0)
        {
            _cacheDelayTime--;
            yield break;
        }

        if (_cacheFallTime<=0)//已经降落完毕了
        {
            yield break;
        }
        
        _sequence?.Play();
        yield return new WaitForSeconds(1f);
        _sequence?.Pause();
        _cacheFallTime--;
        if (_cacheFallTime<=0)
        {
            OnMeteoriteFallDone();
        }
    }

    /// <summary>
    /// 播放完毕时，也就是陨石落地时
    /// </summary>
    private void OnMeteoriteFallDone()
    {
        SfxManager.Instance.PlaySfx("unit_rockBoom");
        meteorite.SetActive(false);
        unitFire.SetFireActive(true);
        unitFire.fireVfx.Play();
        boomVfx.Play();
    }
}