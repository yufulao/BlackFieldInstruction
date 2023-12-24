using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class StageSelectUIModel
{
    private List<StageItemInfo> _stageItemInfoList;
    private int _currentMapId;//cfgMap中有id的含义
    private int _currentAreaId;

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
            StageItemInfo stageInfo = CreateStageItemInfo(stages[i].key, stages[i].scenePath);
            _stageItemInfoList.Add(stageInfo);
        }
        _currentMapId = 0;
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
    public void GetPreMapId()
    {
        _currentMapId--;
        if (_currentMapId<0)
        {
            _currentMapId = ConfigManager.Instance.cfgMap.AllConfigs.Count - 1;
        }

        ResetAreaId();
    }

    /// <summary>
    /// 切换到下一个地图
    /// </summary>
    public void GetNextMapId()
    {
        //Debug.Log(ConfigManager.Instance.cfgMap.AllConfigs.Count);
        _currentMapId++;
        if (_currentMapId>ConfigManager.Instance.cfgMap.AllConfigs.Count-1)
        {
            _currentMapId = 0;
        }

        ResetAreaId();
    }

    /// <summary>
    /// 切换到上一个区域
    /// </summary>
    public void GetPreAreaId()
    {
        _currentAreaId--;
        if (_currentAreaId<0)
        {
            _currentAreaId = ConfigManager.Instance.cfgMap.AllConfigs.Count - 1;
        }
    }

    /// <summary>
    /// 切换到下一个区域
    /// </summary>
    public void GetNextAreaId()
    {
        _currentAreaId++;
        if (_currentAreaId>ConfigManager.Instance.cfgMap[_currentMapId].areaCount-1)
        {
            _currentAreaId = 0;
        }
    }

    /// <summary>
    /// 获取该地图摄像机参数
    /// </summary>
    /// <returns></returns>
    public (Vector3,Vector3, float) GetMapCameraParams()
    {
        _cacheCameraParamsList=ConfigManager.Instance.cfgMap[_currentMapId].cameraPosition;
        Vector3 cameraPosition = new Vector3(_cacheCameraParamsList[0], _cacheCameraParamsList[1], _cacheCameraParamsList[2]);
        Vector3 cameraRotation=new Vector3(_cacheCameraParamsList[3], _cacheCameraParamsList[4], _cacheCameraParamsList[5]);
        float cameraFileOfView = _cacheCameraParamsList[6];
        return (cameraPosition,cameraRotation,cameraFileOfView);
    }

    /// <summary>
    /// 获取该区域摄像机参数
    /// </summary>
    /// <returns></returns>
    public (Vector3,Vector3, float) GetAreaCameraParams()
    {
        switch (_currentAreaId)
        {
            case MapAreaDef.Area0:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentMapId].area0CameraPosition;
                break;
            case MapAreaDef.Area1:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentMapId].area1CameraPosition;
                break;
            case MapAreaDef.Area2:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentMapId].area2CameraPosition;
                break;
            case MapAreaDef.Area3:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentMapId].area3CameraPosition;
                break;
            case MapAreaDef.Area4:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentMapId].area4CameraPosition;
                break;
            case MapAreaDef.Area5:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentMapId].area5CameraPosition;
                break;
            case MapAreaDef.Area6:
                _cacheCameraParamsList = ConfigManager.Instance.cfgMap[_currentMapId].area6CameraPosition;
                break;
        }
        
        Vector3 cameraPosition = new Vector3(_cacheCameraParamsList[0], _cacheCameraParamsList[1], _cacheCameraParamsList[2]);
        Vector3 cameraRotation = new Vector3(_cacheCameraParamsList[3], _cacheCameraParamsList[4], _cacheCameraParamsList[5]);
        float cameraFileOfView = _cacheCameraParamsList[6];
        return (cameraPosition,cameraRotation,cameraFileOfView);
    }
    
    /// <summary>
    /// 生成一个stageItemInfo
    /// </summary>
    /// <param name="stageNameT"></param>
    /// <param name="scenePathT"></param>
    /// <returns></returns>
    private StageItemInfo CreateStageItemInfo(string stageNameT,string scenePathT)
    {
        return new StageItemInfo()
        {
            stageName = stageNameT,
            scenePath = scenePathT
        };
    }

    /// <summary>
    /// 重置区域id
    /// </summary>
    private void ResetAreaId()
    {
        _currentAreaId = 0;
    }
    
}
