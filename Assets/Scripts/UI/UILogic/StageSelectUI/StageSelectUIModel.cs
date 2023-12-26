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
    public (Vector3,Vector3, float) GetHighCameraParams()
    {
        _cacheCameraParamsList=ConfigManager.Instance.cfgMap[_currentHighId].highCameraParams;
        return TranslateCameraParams();
    }
    
    public (Vector3,Vector3, float) GetMidCameraParams()
    {
        _cacheCameraParamsList=ConfigManager.Instance.cfgMap[_currentHighId].midCameraParams;
        return TranslateCameraParams();
    }

    public int GetLowCount()
    {
        return ConfigManager.Instance.cfgMap[_currentHighId].lowCount;
    }

    /// <summary>
    /// 获取该区域摄像机参数
    /// </summary>
    /// <returns></returns>
    public (Vector3,Vector3, float) GetLowCameraParams()
    {
        switch (_currentLowId)
        {
            case MapAreaDef.Area0:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentHighId].lowACameraParams;
                break;
            case MapAreaDef.Area1:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentHighId].lowBCameraParams;
                break;
            case MapAreaDef.Area2:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentHighId].lowCCameraParams;
                break;
            case MapAreaDef.Area3:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentHighId].lowDCameraParams;
                break;
            case MapAreaDef.Area4:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentHighId].lowECameraParams;
                break;
            case MapAreaDef.Area5:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentHighId].lowFCameraParams;
                break;
            case MapAreaDef.Area6:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentHighId].lowGCameraParams;
                break;
        }

        return TranslateCameraParams();
    }

    private (Vector3,Vector3, float) TranslateCameraParams()
    {
        Vector3 cameraPosition = new Vector3(_cacheCameraParamsList[0], _cacheCameraParamsList[1], _cacheCameraParamsList[2]);
        Vector3 cameraRotation = new Vector3(_cacheCameraParamsList[3], _cacheCameraParamsList[4], _cacheCameraParamsList[5]);
        float cameraFileOfView = _cacheCameraParamsList[6];
        return (cameraPosition,cameraRotation,cameraFileOfView);
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
