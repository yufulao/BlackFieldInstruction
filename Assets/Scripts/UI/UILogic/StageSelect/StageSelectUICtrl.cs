using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

public class StageSelectUICtrl : UICtrlBase
{
    private StageSelectUIModel _model;

    [SerializeField] private Transform stageItemContainer;

    public override void OnInit(params object[] param)
    {
        _model = new StageSelectUIModel();
        ReloadModel();
        InitView();
    }

    public override void OpenRoot()
    {
        gameObject.SetActive(true);
    }

    public override void CloseRoot()
    {
        gameObject.SetActive(false);
    }

    protected override void BindEvent()
    {
    }

    /// <summary>
    /// 加载model
    /// </summary>
    private void ReloadModel()
    {
        List<StageItemInfo> stageInfos = new List<StageItemInfo>();
        List<RowCfgStage> stages = ConfigManager.Instance.cfgStage.AllConfigs;
        for (int i = 0; i < stages.Count; i++)
        {
            StageItemInfo stageInfo = _model.CreateStageItemInfo(stages[i].key, stages[i].scenePath);
            stageInfos.Add(stageInfo);
        }

        _model.InitModel(stageInfos);
    }

    /// <summary>
    /// 初始化items列表
    /// </summary>
    private void InitView()
    {
        List<StageItemInfo> stageInfos = _model.GetStageItemInfoList();
        for (int i = 0; i < stageInfos.Count; i++)
        {
            int index = i;
            StageItem item = CreateStageItem();
            item.SetClickBtnOnClick(() =>
            {
                GameManager.Instance.EnterStage(stageInfos[index].stageName, stageInfos[index].scenePath);
                UIManager.Instance.CloseWindows("StageSelectView");
            });
            item.Refresh(stageInfos[index].stageName);
        }
    }

    /// <summary>
    /// 创建一个关卡item
    /// </summary>
    /// <returns></returns>
    private StageItem CreateStageItem()
    {
        return Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgPrefab["StageItem"].prefabPath)
            , stageItemContainer).GetComponent<StageItem>();
    }
}