using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectUIModel
{
    private List<StageItemInfo> _stageItemInfoList;

    /// <summary>
    /// 获取itemInfo列表
    /// </summary>
    /// <returns></returns>
    public List<StageItemInfo> GetStageItemInfoList()
    {
        return _stageItemInfoList;
    }
    
    /// <summary>
    /// 初始化model
    /// </summary>
    /// <param name="stageItemInfoList"></param>
    public void InitModel(List<StageItemInfo> stageItemInfoList)
    {
        _stageItemInfoList = stageItemInfoList;
    }
    
    /// <summary>
    /// 生成一个stageItemInfo
    /// </summary>
    /// <param name="stageNameT"></param>
    /// <param name="scenePathT"></param>
    /// <returns></returns>
    public StageItemInfo CreateStageItemInfo(string stageNameT,string scenePathT)
    {
        return new StageItemInfo()
        {
            stageName = stageNameT,
            scenePath = scenePathT
        };
    }
}
