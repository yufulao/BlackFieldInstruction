using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private readonly List<IMonoManager> _managerList = new List<IMonoManager>();
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        
        _managerList.Add(EventManager.Instance);
        _managerList.Add(AssetManager.Instance);
        _managerList.Add(InputManager.Instance);
        _managerList.Add(FsmManager.Instance);
        _managerList.Add(BgmManager.Instance);
        _managerList.Add(SfxManager.Instance);
        _managerList.Add(SceneManager.Instance);

        foreach (var manager in _managerList)
        {
            manager.OnInit();
        }

        //测试
        //StartCoroutine(BgmManager.Instance.PlayBgmFadeDelay("测试bgm",0f, 0f, 0f));
        //SfxManager.Instance.PlaySfx("测试sfx",1f);
        //EventManager.Instance.AddListener(EventName.Click,()=>{Debug.Log("Click");});
        SceneManager.Instance.ChangeSceneAsync("TestScene");
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
