using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private List<IMonoManager> _managerList = new List<IMonoManager>();
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        
        _managerList.Add(FsmManager.Instance);
        _managerList.Add(BgmManager.Instance);
        _managerList.Add(SfxManager.Instance);

        foreach (var manager in _managerList)
        {
            manager.OnInit();
        }
    }

    private void Update()
    {
        foreach (var manager in _managerList)
        {
            manager.Update();
        }
    }

    private void FixedUpdate()
    {
        foreach (var manager in _managerList)
        {
            manager.FixedUpdate();
        }
    }

    private void LateUpdate()
    {
        foreach (var manager in _managerList)
        {
            manager.LateUpdate();
        }
    }
    
    private void OnDestroy()
    {
        for (var i = _managerList.Count - 1; i >= 0; i--)
        {
            _managerList[i].OnClear();
        }
    }
}
