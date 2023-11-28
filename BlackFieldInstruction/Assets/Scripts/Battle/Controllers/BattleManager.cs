using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BattleManager:MonoSingleton<BattleManager>
{
    private CfgStage _cfgStage;

    /// <summary>
    /// 进入battle场景
    /// </summary>
    /// <param name="stageName"></param>
    public void EnterStageScene(string stageName)
    {
        ReloadGridMvc(stageName);
    }

    /// <summary>
    /// 重新加载新的一整套网格mvc
    /// </summary>
    /// <param name="stageName"></param>
    private void ReloadGridMvc(string stageName)
    {
        _cfgStage=ConfigManager.Instance.cfgStage;
        //理论上，协程结束后场景已经切换，所以GridManager会被替换成新场景中的instance
        GridManager.Instance.ReloadGridManager(_cfgStage[stageName]);//重新获取场景中的model和view
    }
    
}
