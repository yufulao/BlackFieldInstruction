using System;
using System.Collections;
using System.Collections.Generic;
using FlatKit;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoSingleton<GameManager>
{
    private readonly List<IMonoManager> _managerList = new List<IMonoManager>();
    public bool test;//测试模式
    public bool crack;//破解版

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
        _managerList.Add(PipelineManager.Instance);

        foreach (var manager in _managerList)
        {
            manager.OnInit();
        }
    }

    private void Start()
    {
        if (test)
        {
            IsTest();
        }

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
        //SaveManager.SetFloat("TestFloat",0.5f);
        //Debug.Log(SaveManager.GetFloat("TestFloat", 0.1f));
        ReturnToTitle();
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    public void ReturnToTitle()
    {
        StartCoroutine(IReturnToTitle());
    }

    private void IsTest()
    {
        Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgUI["IngameDebugView"].uiPath)
            , UIManager.Instance.GetUIRoot().Find("NormalLayer"));
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    private IEnumerator IReturnToTitle()
    {
        BattleManager.Instance.ForceQuitBattle();
        UIManager.Instance.CloseAllWindows();
        UIManager.Instance.OpenWindow("LoadingView");
        yield return BgmManager.Instance.StopBgmFadeDelay(0f, 0.4f);
        yield return new WaitForSeconds(0.5f);
        SetTimeScale(1f);
        yield return SceneManager.Instance.ChangeSceneAsync("MainScene");
        CameraManager.Instance.ResetObjCamera();
        UIManager.Instance.OpenWindow("StageSelectView");
        GC.Collect();
        UIManager.Instance.CloseWindows("LoadingView");
        CameraManager.Instance.GetObjCamera().transform.GetComponent<AutoLoadPipelineAsset>().SetPipeline(PipelineManager.Instance.mainScenePipelineAsset);
        yield return BgmManager.Instance.PlayBgmFadeDelay("MainScene", 0f, 0f, 0.5f, 0.5f);
    }

    /// <summary>
    /// 进入关卡
    /// </summary>
    /// <param name="stageName"></param>
    public void EnterStage(string stageName)
    {
        StartCoroutine(IEnterStage(ConfigManager.Instance.cfgStage[stageName]));
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }

    /// <summary>
    /// 设置时间速率
    /// </summary>
    /// <param name="timeScale"></param>
    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    /// <summary>
    /// 进入游戏关卡
    /// </summary>
    /// <param name="rowCfgStage"></param>
    /// <returns></returns>
    private IEnumerator IEnterStage(RowCfgStage rowCfgStage)
    {
        UIManager.Instance.OpenWindow("LoadingView"); //加载界面
        yield return BgmManager.Instance.StopBgmFadeDelay(0f, 0.4f); //关闭bgm
        yield return new WaitForSeconds(0.5f);
        yield return SceneManager.Instance.ChangeSceneAsync(rowCfgStage.scenePath); //切换场景
        BattleManager.Instance.EnterStageScene(rowCfgStage); //battleManager切换状态机
        StartCoroutine(CameraManager.Instance.MoveObjCamera(rowCfgStage.stageCamera)); //切换摄像机
        CameraManager.Instance.GetObjCamera().transform.GetComponent<AutoLoadPipelineAsset>().SetPipeline(PipelineManager.Instance.stagePipelineAsset);
        //Debug.Log(PipelineManager.Instance.stagePipelineAsset.defaultShader.name);
        SfxManager.Instance.PlaySfx("level_generate"); //播放进入关卡音效
        GC.Collect(); //清gc
        UIManager.Instance.CloseWindows("LoadingView"); //关闭加载界面
        yield return BgmManager.Instance.PlayBgmFadeDelay(rowCfgStage.stageBgm, 0f, 0f, 0.5f, 1f); //播放关卡bgm
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