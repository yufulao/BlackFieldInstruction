using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class StageSelectUIModel
{
    private List<StageItemInfo> _stageItemInfoList;
    private int _currentHighId;//cfgMap中有id的含义
    private int _currentLowId;

    private List<float> _cacheCameraParamsList = new List<float>();

    /// <summary>
    /// 初始化model
    /// </summary>
    public void InitModel()
    {
        _stageItemInfoList = new List<StageItemInfo>();
        List<RowCfgStage> stages = ConfigManager.Instance.cfgStage.AllConfigs;
        for (int i = 0; i < stages.Count; i++)
        {
            StageItemInfo stageInfo = CreateStageItemInfo(stages[i].key);
            //Debug.Log(stages[i].key+"  "+stageInfo.stageName);
            _stageItemInfoList.Add(stageInfo);
        }
        _currentHighId = 0;
    }

    /// <summary>
    /// 通过传入StageItem的stageCfgKey获取关卡信息
    /// </summary>
    /// <param name="stageCfgKey"></param>
    /// <returns></returns>
    public StageItemInfo GetStageInfo(string stageCfgKey)
    {
        for (int i = 0; i < _stageItemInfoList.Count; i++)
        {
            if (stageCfgKey==_stageItemInfoList[i].stageName)
            {
                return _stageItemInfoList[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 切换到上一个地图
    /// </summary>
    public void GetPreHighId()
    {
        _currentHighId--;
        if (_currentHighId<0)
        {
            _currentHighId = ConfigManager.Instance.cfgMap.AllConfigs.Count - 1;
        }

        ResetLowId();
    }

    /// <summary>
    /// 切换到下一个地图
    /// </summary>
    public void GetNextHighId()
    {
        //Debug.Log(ConfigManager.Instance.cfgMap.AllConfigs.Count);
        _currentHighId++;
        if (_currentHighId>ConfigManager.Instance.cfgMap.AllConfigs.Count-1)
        {
            _currentHighId = 0;
        }

        ResetLowId();
    }

    /// <summary>
    /// 切换到上一个区域
    /// </summary>
    public void GetPreLowId()
    {
        _currentLowId--;
        if (_currentLowId<0)
        {
            _currentLowId = ConfigManager.Instance.cfgMap.AllConfigs.Count - 1;
        }
    }

    /// <summary>
    /// 切换到下一个区域
    /// </summary>
    public void GetNextLowId()
    {
        _currentLowId++;
        if (_currentLowId>ConfigManager.Instance.cfgMap[_currentHighId].lowCount-1)
        {
            _currentLowId = 0;
        }
    }

    /// <summary>
    /// 获取该地图摄像机参数
    /// </summary>
    /// <returns></returns>
    public string GetHighCamera()
    {
        return ConfigManager.Instance.cfgMap[_currentHighId].highCamera;
    }
    
    public string GetMidCamera()
    {
        return ConfigManager.Instance.cfgMap[_currentHighId].midCamera;
    }

    public int GetLowCount()
    {
        return ConfigManager.Instance.cfgMap[_currentHighId].lowCount;
    }

    /// <summary>
    /// 获取该区域摄像机参数
    /// </summary>
    /// <returns></returns>
    public string GetLowCamera()
    {
        switch (_currentLowId)
        {
            case MapAreaDef.Area0:
                return ConfigManager.Instance.cfgMap[_currentHighId].lowACamera;
            case MapAreaDef.Area1:
                return ConfigManager.Instance.cfgMap[_currentHighId].lowBCamera;
            case MapAreaDef.Area2:
                return ConfigManager.Instance.cfgMap[_currentHighId].lowCCamera;
            case MapAreaDef.Area3:
                return ConfigManager.Instance.cfgMap[_currentHighId].lowDCamera;
            case MapAreaDef.Area4:
                return ConfigManager.Instance.cfgMap[_currentHighId].lowECamera;
            case MapAreaDef.Area5:
                return ConfigManager.Instance.cfgMap[_currentHighId].lowFCamera;
            case MapAreaDef.Area6:
                return ConfigManager.Instance.cfgMap[_currentHighId].lowGCamera;
        }

        return null;
    }

    /// <summary>
    /// 生成一个stageItemInfo
    /// </summary>
    /// <param name="stageNameT"></param>
    /// <returns></returns>
    private StageItemInfo CreateStageItemInfo(string stageNameT)
    {
        return new StageItemInfo()
        {
            stageName = stageNameT,
        };
    }

    /// <summary>
    /// 重置区域id
    /// </summary>
    private void ResetLowId()
    {
        _currentLowId = 0;
    }
    
}
