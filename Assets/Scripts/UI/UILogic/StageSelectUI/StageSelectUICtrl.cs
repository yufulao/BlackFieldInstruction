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
    [SerializeField] private GameObject highUIFrame;
    [SerializeField] private GameObject midUIFrame;
    [SerializeField] private GameObject lowUIFrame;
    [SerializeField] private Button preHighBtn;
    [SerializeField] private Button nextHighBtn;
    [SerializeField] private Button highEnterMask;
    [SerializeField] private Button midEnterMask;
    [SerializeField] private Button midBackBtn;
    [SerializeField] private Button preLowBtn;
    [SerializeField] private Button nextLowBtn;
    [SerializeField] private Button lowBackBtn;

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
        _fsm.ChangeFsmState(typeof(HighSelectState));
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
        preHighBtn.onClick.AddListener(OnPreHighBtnClick);
        nextHighBtn.onClick.AddListener(OnNextHighBtnClick);
        highEnterMask.onClick.AddListener(OnHighEnterMaskClick);
        midEnterMask.onClick.AddListener(OnMidEnterMaskClick);
        midBackBtn.onClick.AddListener(OnMidBackBtnClick);
        preLowBtn.onClick.AddListener(OnPreLowBtnClick);
        nextLowBtn.onClick.AddListener(OnNextLowBtnClick);
        lowBackBtn.onClick.AddListener(OnLowBackBtnClick);
    }

    /// <summary>
    /// 地图选择阶段
    /// </summary>
    public void HighSelectStateEnter()
    {
        highUIFrame.SetActive(true);
        midUIFrame.SetActive(false);
        lowUIFrame.SetActive(false);
    }

    /// <summary>
    /// 切换地图中
    /// </summary>
    public void HighChangingStateEnter()
    {
        CloseAllFrame();
    }

    /// <summary>
    /// 地图选择阶段切换到区域选择阶段
    /// </summary>
    public void HighToMidStateEnter()
    {
        CloseAllFrame();
    }
    
    public void MidStateEnter()
    {
        highUIFrame.SetActive(false);
        midUIFrame.SetActive(true);
        lowUIFrame.SetActive(false);
    }
    
    public void MidToHighStateEnter()
    {
        CloseAllFrame();
    }
    
    public void MidToLowStateEnter()
    {
        CloseAllFrame();
    }

    /// <summary>
    /// 区域选择阶段
    /// </summary>
    public void LowSelectStateEnter()
    {
        highUIFrame.SetActive(false);
        midUIFrame.SetActive(false);
        lowUIFrame.SetActive(true);
    }

    /// <summary>
    /// 区域选择阶段，检测点击
    /// </summary>
    public void LowSelectStateUpdate()
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
    public void LowChangingStateEnter()
    {
        CloseAllFrame();
    }

    /// <summary>
    /// 从区域选择阶段切换到地图选择阶段
    /// </summary>
    public void LowToMidStateEnter()
    {
        CloseAllFrame();
    }

    /// <summary>
    /// 点击stageItem这个obj
    /// </summary>
    /// <param name="stageItem"></param>
    private void OnStageItemClick(StageItem stageItem)
    {
        StageItemInfo stageInfo = _model.GetStageInfo(stageItem.stageCfgKey);
        GameManager.Instance.EnterStage(stageInfo.stageName);
        UIManager.Instance.CloseWindows("StageSelectView");
    }

    /// <summary>
    /// 点击进入地图btn
    /// </summary>
    private void OnHighEnterMaskClick()
    {
        StartCoroutine(HighToMid());
    }

    //点击上一个地图btn
    private void OnPreHighBtnClick()
    {
        _model.GetPreHighId();
        StartCoroutine(UpdateHigh(0f));
    }

    /// <summary>
    /// 点击下一个地图btn
    /// </summary>
    private void OnNextHighBtnClick()
    {
        _model.GetNextHighId();
        StartCoroutine(UpdateHigh(0f));
    }
    
    private void OnMidEnterMaskClick()
    {
        if (_model.GetLowCount()<=0)
        {
            return;
        }
        StartCoroutine(MidToLow());
    }
    
    private void OnMidBackBtnClick()
    {
        StartCoroutine(MidToHigh());
    }

    /// <summary>
    /// 点击上一个区域btn
    /// </summary>
    private void OnPreLowBtnClick()
    {
        _model.GetPreLowId();
        StartCoroutine(UpdateLow(0.5f));
    }

    /// <summary>
    /// 点击下一个区域btn
    /// </summary>
    private void OnNextLowBtnClick()
    {
        _model.GetNextLowId();
        StartCoroutine(UpdateLow(0.5f));
    }

    /// <summary>
    /// 点击返回地图btn
    /// </summary>
    private void OnLowBackBtnClick()
    {
        StartCoroutine(LowToMid());
    }
    
    private IEnumerator HighToMid()
    {
        _fsm.ChangeFsmState(typeof(HighToMidState));
        yield return UpdateMid(0.5f);
    }
    
    private IEnumerator MidToHigh()
    {
        _fsm.ChangeFsmState(typeof(MidToHighState));
        yield return UpdateHigh(0.5f);
    }

    private IEnumerator MidToLow()
    {
        _fsm.ChangeFsmState(typeof(MidToLowState));
        yield return UpdateLow(0.5f);
    }

    private IEnumerator LowToMid()
    {
        _fsm.ChangeFsmState(typeof(LowToMidState));
        yield return UpdateMid(0.5f);
    }

    /// <summary>
    /// 初始化View
    /// </summary>
    private void InitView()
    {
        StartCoroutine(UpdateHigh(1f));
    }

    /// <summary>
    /// 刷新地图
    /// </summary>
    /// <param name="during"></param>
    /// <returns></returns>
    private IEnumerator UpdateHigh(float during)
    {
        _fsm.ChangeFsmState(typeof(HighChangingState));
        var cameraParams = _model.GetHighCameraParams();
        yield return CameraManager.Instance.MoveObjCamera(cameraParams.Item1, cameraParams.Item2, cameraParams.Item3, during);
        //Debug.Log(cameraParams.Item1 + "  " + cameraParams.Item2 + "  " + cameraParams.Item3);
        _fsm.ChangeFsmState(typeof(HighSelectState));
    }
    
    private IEnumerator UpdateMid(float during)
    {
        var cameraParams = _model.GetMidCameraParams();
        yield return CameraManager.Instance.MoveObjCamera(cameraParams.Item1, cameraParams.Item2, cameraParams.Item3, during);
        //Debug.Log(cameraParams.Item1 + "  " + cameraParams.Item2 + "  " + cameraParams.Item3);
        _fsm.ChangeFsmState(typeof(MidState));
    }

    /// <summary>
    /// 刷新区域
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateLow(float during)
    {
        _fsm.ChangeFsmState(typeof(LowChangingState));
        var cameraParams = _model.GetLowCameraParams();
        yield return CameraManager.Instance.MoveObjCamera(cameraParams.Item1, cameraParams.Item2, cameraParams.Item3, during);
        //Debug.Log(cameraParams.Item1 + "  " + cameraParams.Item2 + "  " + cameraParams.Item3);
        _fsm.ChangeFsmState(typeof(LowSelectState));
    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    private void InitFsm()
    {
        _fsm = new FsmComponent<StageSelectUICtrl>(this);
        Dictionary<Type, IFsmComponentState<StageSelectUICtrl>> fsmStates = new Dictionary<Type, IFsmComponentState<StageSelectUICtrl>>();
        fsmStates.Add(typeof(HighChangingState), new HighChangingState());
        fsmStates.Add(typeof(HighSelectState), new HighSelectState());
        fsmStates.Add(typeof(HighToMidState), new HighToMidState());
        fsmStates.Add(typeof(MidState), new MidState());
        fsmStates.Add(typeof(MidToHighState), new MidToHighState());
        fsmStates.Add(typeof(MidToLowState), new MidToLowState());
        fsmStates.Add(typeof(LowChangingState), new LowChangingState());
        fsmStates.Add(typeof(LowSelectState), new LowSelectState());
        fsmStates.Add(typeof(LowToMidState), new LowToMidState());
        _fsm.SetFsm(fsmStates);
        //fsm此时处于挂起状态，没有state
    }

    private void CloseAllFrame()
    {
        highUIFrame.SetActive(false);
        midUIFrame.SetActive(false);
        lowUIFrame.SetActive(false);
    }

    private void Update()
    {
        _fsm?.OnUpdate();
    }
}