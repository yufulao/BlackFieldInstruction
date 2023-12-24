using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using MouseButton = UnityEngine.InputSystem.LowLevel.MouseButton;

public class StageSelectUICtrl : UICtrlBase
{
    [SerializeField] private GameObject mapUIFrame;
    [SerializeField] private GameObject areaUIFrame;
    [SerializeField] private Button preMapBtn;
    [SerializeField] private Button nextMapBtn;
    [SerializeField] private Button mapEnterMask;
    [SerializeField] private Button preAreaBtn;
    [SerializeField] private Button nextAreaBtn;
    [SerializeField] private Button areaBackBtn;

    private StageSelectUIModel _model;
    private FsmComponent<StageSelectUICtrl> _fsm;

    private Ray _ray;
    private RaycastHit _hit;
    private GameObject _obj;


    public override void OnInit(params object[] param)
    {
        InitFsm();
        _model = new StageSelectUIModel();
        _model.InitModel();
        BindEvent();
        _fsm.ChangeFsmState(typeof(MapSelectState));
    }

    public override void OpenRoot()
    {
        gameObject.SetActive(true);
        InitView();
    }

    public override void CloseRoot()
    {
        gameObject.SetActive(false);
    }

    protected override void BindEvent()
    {
        preMapBtn.onClick.AddListener(OnPreMapBtnClick);
        nextMapBtn.onClick.AddListener(OnNextMapBtnClick);
        mapEnterMask.onClick.AddListener(OnMapEnterMaskClick);
        preAreaBtn.onClick.AddListener(OnPreAreaBtnClick);
        nextAreaBtn.onClick.AddListener(OnNextAreaBtnClick);
        areaBackBtn.onClick.AddListener(OnAreaBackBtnClick);
    }

    /// <summary>
    /// 地图选择阶段
    /// </summary>
    public void MapSelectStateEnter()
    {
        mapUIFrame.SetActive(true);
        areaUIFrame.SetActive(false);
    }

    /// <summary>
    /// 切换地图中
    /// </summary>
    public void MapChangingStateEnter()
    {
        mapUIFrame.SetActive(false);
        areaUIFrame.SetActive(false);
    }

    /// <summary>
    /// 地图选择阶段切换到区域选择阶段
    /// </summary>
    public void MapToAreaStateEnter()
    {
        mapUIFrame.SetActive(false);
        areaUIFrame.SetActive(false);
    }

    /// <summary>
    /// 区域选择阶段
    /// </summary>
    public void AreaSelectStateEnter()
    {
        mapUIFrame.SetActive(false);
        areaUIFrame.SetActive(true);
    }

    /// <summary>
    /// 区域选择阶段，检测点击
    /// </summary>
    public void AreaSelectStateUpdate()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _ray = CameraManager.Instance.GetObjCamera().ScreenPointToRay(Mouse.current.position.value);
            if (Physics.Raycast(_ray, out _hit))
            {
                GameObject result = _hit.collider.gameObject;
                if (result.name.Contains("StageItem"))
                {
                    OnStageItemClick(result.GetComponent<StageItem>());
                }
            }
        }
    }

    /// <summary>
    /// 切换区域阶段
    /// </summary>
    public void AreaChangingStateEnter()
    {
        mapUIFrame.SetActive(false);
        areaUIFrame.SetActive(false);
    }

    /// <summary>
    /// 从区域选择阶段切换到地图选择阶段
    /// </summary>
    public void AreaToMapStateEnter()
    {
        mapUIFrame.SetActive(false);
        areaUIFrame.SetActive(false);
    }

    /// <summary>
    /// 点击stageItem这个obj
    /// </summary>
    /// <param name="stageItem"></param>
    private void OnStageItemClick(StageItem stageItem)
    {
        StageItemInfo stageInfo = _model.GetStageInfo(stageItem.stageCfgKey);
        GameManager.Instance.EnterStage(stageInfo.stageName, stageInfo.scenePath);
        UIManager.Instance.CloseWindows("StageSelectView");
    }

    /// <summary>
    /// 点击进入地图btn
    /// </summary>
    private void OnMapEnterMaskClick()
    {
        StartCoroutine(MapToArea());
    }

    //点击上一个地图btn
    private void OnPreMapBtnClick()
    {
        _model.GetPreMapId();
        StartCoroutine(UpdateMap(0f));
    }

    /// <summary>
    /// 点击下一个地图btn
    /// </summary>
    private void OnNextMapBtnClick()
    {
        _model.GetNextMapId();
        StartCoroutine(UpdateMap(0f));
    }

    /// <summary>
    /// 点击上一个区域btn
    /// </summary>
    private void OnPreAreaBtnClick()
    {
        _model.GetPreAreaId();
        StartCoroutine(UpdateArea(0.5f));
    }

    /// <summary>
    /// 点击下一个区域btn
    /// </summary>
    private void OnNextAreaBtnClick()
    {
        _model.GetNextAreaId();
        StartCoroutine(UpdateArea(0.5f));
    }

    /// <summary>
    /// 点击返回地图btn
    /// </summary>
    private void OnAreaBackBtnClick()
    {
        StartCoroutine(AreaToMap());
    }
    
    private IEnumerator MapToArea()
    {
        _fsm.ChangeFsmState(typeof(MapToAreaState));
        yield return UpdateArea(0.5f);
    }
    
    private IEnumerator AreaToMap()
    {
        _fsm.ChangeFsmState(typeof(AreaToMapState));
        yield return UpdateMap(0.5f);
    }

    /// <summary>
    /// 初始化View
    /// </summary>
    private void InitView()
    {
        StartCoroutine(UpdateMap(0f));
    }

    /// <summary>
    /// 刷新地图
    /// </summary>
    /// <param name="during"></param>
    /// <returns></returns>
    private IEnumerator UpdateMap(float during)
    {
        _fsm.ChangeFsmState(typeof(MapChangingState));
        var cameraParams = _model.GetMapCameraParams();
        yield return CameraManager.Instance.MoveObjCamera(cameraParams.Item1, cameraParams.Item2, cameraParams.Item3, during);
        //Debug.Log(cameraParams.Item1 + "  " + cameraParams.Item2 + "  " + cameraParams.Item3);
        _fsm.ChangeFsmState(typeof(MapSelectState));
    }

    /// <summary>
    /// 刷新区域
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateArea(float during)
    {
        _fsm.ChangeFsmState(typeof(AreaChangingState));
        var cameraParams = _model.GetAreaCameraParams();
        yield return CameraManager.Instance.MoveObjCamera(cameraParams.Item1, cameraParams.Item2, cameraParams.Item3, during);
        //Debug.Log(cameraParams.Item1 + "  " + cameraParams.Item2 + "  " + cameraParams.Item3);
        _fsm.ChangeFsmState(typeof(AreaSelectState));
    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    private void InitFsm()
    {
        _fsm = new FsmComponent<StageSelectUICtrl>(this);
        Dictionary<Type, IFsmComponentState<StageSelectUICtrl>> fsmStates = new Dictionary<Type, IFsmComponentState<StageSelectUICtrl>>();
        fsmStates.Add(typeof(MapSelectState), new MapSelectState());
        fsmStates.Add(typeof(MapChangingState), new MapChangingState());
        fsmStates.Add(typeof(MapToAreaState), new MapToAreaState());
        fsmStates.Add(typeof(AreaSelectState), new AreaSelectState());
        fsmStates.Add(typeof(AreaChangingState), new AreaChangingState());
        fsmStates.Add(typeof(AreaToMapState), new AreaToMapState());
        _fsm.SetFsm(fsmStates);
        //fsm此时处于挂起状态，没有state
    }

    private void Update()
    {
        _fsm?.OnUpdate();
    }
}