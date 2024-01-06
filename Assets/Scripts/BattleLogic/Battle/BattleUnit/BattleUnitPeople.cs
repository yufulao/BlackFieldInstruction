using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rabi;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleUnitPeople : BattleUnit
{
    [SerializeField] private bool originalActive;
    [SerializeField] private GameObject peopleObj;
    [Range(0f, 1f)] [SerializeField] private float textRate;
    [SerializeField] private Text flyText;

    [HideInInspector] public bool currentActive;
    private float _cacheTextTimer;
    private List<string> _textListBeforeExecute;
    private List<string> _textListDuringExecute;
    private List<string> _textListOnDie;
    private List<string> _textListOnHelp;

    private void Update()
    {
        _cacheTextTimer -= Time.deltaTime;
        if (currentActive && _cacheTextTimer < 0f)
        {
            _cacheTextTimer = 1f;
            if (Random.Range(0f, 1f) < textRate)
            {
                TextPop(_textListBeforeExecute[Random.Range(0, _textListBeforeExecute.Count)]);
                _cacheTextTimer += 1f;
            }
        }
    }

    public override void OnUnitInit()
    {
        base.OnUnitInit();
        _textListBeforeExecute = ConfigManager.Instance.cfgBattleUnitPeopleText["BeforeExecute"].textList;
        _textListDuringExecute=ConfigManager.Instance.cfgBattleUnitPeopleText["DuringExecute"].textList;
        _textListOnDie = ConfigManager.Instance.cfgBattleUnitPeopleText["OnDie"].textList;
        _textListOnHelp = ConfigManager.Instance.cfgBattleUnitPeopleText["OnHelp"].textList;
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
    }

    public void PeopleHelp()
    {
        if (currentActive)
        {
            SfxManager.Instance.PlaySfx("unit_peopleHelp");
            TextPopOnHelp();
            SetPeopleActive(false);
        }
    }

    public void PeopleDie()
    {
        if (currentActive)
        {
            SfxManager.Instance.PlaySfx("unit_peopleDie");
            TextPopOnDie();
            SetPeopleActive(false);
        }
    }

    /// <summary>
    /// 人群获救时飘字
    /// </summary>
    private void TextPopOnHelp()
    {
        TextPop(_textListOnHelp[Random.Range(0, _textListOnHelp.Count)]);
    }

    /// <summary>
    /// 人群死亡时飘字
    /// </summary>
    private void TextPopOnDie()
    {
        TextPop(_textListOnDie[Random.Range(0, _textListOnDie.Count)]);
    }

    /// <summary>
    /// 设置人群的状态
    /// </summary>
    /// <param name="active"></param>
    private void SetPeopleActive(bool active)
    {
        currentActive = active;
        peopleObj.SetActive(currentActive);
    }

    /// <summary>
    /// 重置unit
    /// </summary>
    private void ResetAll()
    {
        SetPeopleActive(originalActive);
        flyText.text = "";
    }

    private void TextPop(string text)
    {
        flyText.text = text;
        Utils.TextFly(flyText, CameraManager.Instance.GetObjCamera().WorldToScreenPoint(transform.position));
    }
}