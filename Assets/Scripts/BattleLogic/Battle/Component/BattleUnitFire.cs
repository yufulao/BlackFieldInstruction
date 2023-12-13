using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitFire : BattleUnit
{
    [SerializeField] private bool originalActive; //初始是否在燃烧
    [SerializeField] private ParticleSystem fireVfx; //初始是否在燃烧
    [HideInInspector] public bool currentActive; //当前是否在燃烧

    

    public override void OnUnitReset()
    {
        base.OnUnitReset();
        currentActive = originalActive;
        gameObject.SetActive(currentActive);
        SetVfxActive(currentActive);
    }

    public void SetFireActive(bool active)
    {
        currentActive = active;
        gameObject.SetActive(active);
        SetVfxActive(active);
    }

    private void SetVfxActive(bool active)
    {
        if (active)
        {
            fireVfx.Play();
            return;
        }
        fireVfx.Stop();
    }
}