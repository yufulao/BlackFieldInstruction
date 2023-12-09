using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectUIModel
{
    private List<StageItemInfo> _stageItemInfoList;

    public List<StageItemInfo> GetStageItemInfoList()
    {
        return _stageItemInfoList;
    }
    
    public void InitModel(List<StageItemInfo> stageItemInfoList)
    {
        _stageItemInfoList = stageItemInfoList;
    }
    
    public StageItemInfo CreateStageItemInfo(string stageNameT,string scenePathT)
    {
        return new StageItemInfo()
        {
            stageName = stageNameT,
            scenePath = scenePathT
        };
    }
}
