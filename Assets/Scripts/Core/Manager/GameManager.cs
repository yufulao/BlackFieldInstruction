using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private readonly List<IMonoManager> _managerList = new List<IMonoManager>();
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        Application.targetFrameRate = 120;
        
        _managerList.Add(EventManager.Instance);
        _managerList.Add(AssetManager.Instance);
        _managerList.Add(InputManager.Instance);
        _managerList.Add(FsmManager.Instance);
        _managerList.Add(BgmManager.Instance);
        _managerList.Add(SfxManager.Instance);
        _managerList.Add(SceneManager.Instance);
        _managerList.Add(UIManager.Instance);
        _managerList.Add(CameraManager.Instance);
        _managerList.Add(BattleManager.Instance);

        foreach (var manager in _managerList)
        {
            manager.OnInit();
        }
    }

    private void Start()
    {
        //测试
        //StartCoroutine(BgmManager.Instance.PlayBgmFadeDelay("TestBgm",0f, 0f, 0f));
        //StartCoroutine(SfxManager.Instance.PlaySfx("TestSfx",1f));
        //EventManager.Instance.AddListener(EventName.Click,()=>{Debug.Log("Click");});
        //Debug.Log(ConfigManager.Instance.cfgBgm["TestBgm"].key);
        // StartCoroutine(SceneManager.Instance.ChangeSceneAsync("StageTest",(sceneInstance)=>
        // {
        //     BattleManager.Instance.OnInit();
        //     BattleManager.Instance.EnterStageScene("StageTest");
        // }));
        //GameObject obj = CommandManager.Instance.CreatWaitingObj();
        StartCoroutine(TestStart());
        //SaveManager.SetFloat("TestFloat",0.5f);
        //Debug.Log(SaveManager.GetFloat("TestFloat", 0.1f));
    }

    private IEnumerator TestStart()
    {
        yield return SceneManager.Instance.ChangeSceneAsync("MainScene");
        UIManager.Instance.OpenWindow("StageSelectView");
    }

    public void EnterStage(string stageName)
    {
        StartCoroutine(IEnterStage(stageName,ConfigManager.Instance.cfgStage[stageName].scenePath));
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    IEnumerator IEnterStage(string stageName,string scenePath)
    {
        yield return SceneManager.Instance.ChangeSceneAsync(scenePath);
        BattleManager.Instance.EnterStageScene(stageName);
        GC.Collect();
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