using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rabi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using MouseButton = UnityEngine.InputSystem.LowLevel.MouseButton;
using Sequence = DG.Tweening.Sequence;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class StageSelectUICtrl : UICtrlBase
{
    [SerializeField] private GameObject highUIFrame;
    [SerializeField] private GameObject midUIFrame;
    [SerializeField] private GameObject lowUIFrame;
    [SerializeField] private Button preHighBtn;
    [SerializeField] private Button nextHighBtn;
    [SerializeField] private Button highEnterMask;
    [SerializeField] private Button highEnterBtn;
    [SerializeField] private Button midEnterMask;
    [SerializeField] private Button midEnterBtn;
    [SerializeField] private Button midBackBtn;
    [SerializeField] private Button preLowBtn;
    [SerializeField] private Button nextLowBtn;
    [SerializeField] private Button lowBackBtn;
    [SerializeField] private Text lockStageItemClickText;

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
        ResetView();
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
        highEnterBtn.onClick.AddListener(OnHighEnterMaskClick);
        midEnterMask.onClick.AddListener(OnMidEnterMaskClick);
        midEnterBtn.onClick.AddListener(OnMidEnterMaskClick);
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
        if (Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckRayToStageItem(Mouse.current.position.value);
        }

        if (Touchscreen.current != null && Touchscreen.current.press.wasPressedThisFrame)
        {
            CheckRayToStageItem(Touchscreen.current.position.ReadValue());
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

    private void CheckRayToStageItem(Vector3 clickPosition)
    {
        _ray = CameraManager.Instance.GetObjCamera().ScreenPointToRay(clickPosition);
        if (Physics.Raycast(_ray, out _hit))
        {
            GameObject result = _hit.collider.gameObject;
            if (result.name.Contains("StageItem"))
            {
                OnStageItemClick(result.GetComponent<StageItem>());
            }
        }
    }

    /// <summary>
    /// 点击stageItem这个obj
    /// </summary>
    /// <param name="stageItem"></param>
    private void OnStageItemClick(StageItem stageItem)
    {
        SfxManager.Instance.PlaySfx("stage_stageEnter");
        StageItemInfo stageInfo = _model.GetStageInfo(stageItem.stageCfgKey);
        if (SaveManager.GetInt(stageItem.stageCfgKey, 0) == 1)//已解锁
        {
            GameManager.Instance.EnterStage(stageInfo.stageName);
            UIManager.Instance.CloseWindows("StageSelectView");
            return;
        }
        Utils.TextFly(lockStageItemClickText, CameraManager.Instance.GetObjCamera().WorldToScreenPoint(stageItem.transform.position));
    }

    /// <summary>
    /// 点击进入地图btn
    /// </summary>
    private void OnHighEnterMaskClick()
    {
        SfxManager.Instance.PlaySfx("stage_cameraEnter");
        StartCoroutine(HighToMid());
    }

    //点击上一个地图btn
    private void OnPreHighBtnClick()
    {
        SfxManager.Instance.PlaySfx("stage_mapSwitch");
        _model.GetPreHighId();
        StartCoroutine(UpdateHigh(0f));
    }

    /// <summary>
    /// 点击下一个地图btn
    /// </summary>
    private void OnNextHighBtnClick()
    {
        SfxManager.Instance.PlaySfx("stage_mapSwitch");
        _model.GetNextHighId();
        StartCoroutine(UpdateHigh(0f));
    }
    
    private void OnMidEnterMaskClick()
    {
        if (_model.GetLowCount()<=0)
        {
            return;
        }
        SfxManager.Instance.PlaySfx("stage_cameraEnter");
        StartCoroutine(MidToLow());
    }
    
    private void OnMidBackBtnClick()
    {
        SfxManager.Instance.PlaySfx("stage_cameraBack");
        StartCoroutine(MidToHigh());
    }

    /// <summary>
    /// 点击上一个区域btn
    /// </summary>
    private void OnPreLowBtnClick()
    {
        SfxManager.Instance.PlaySfx("stage_mapSwitch");
        _model.GetPreLowId();
        StartCoroutine(UpdateLow(0.5f));
    }

    /// <summary>
    /// 点击下一个区域btn
    /// </summary>
    private void OnNextLowBtnClick()
    {
        SfxManager.Instance.PlaySfx("stage_mapSwitch");
        _model.GetNextLowId();
        StartCoroutine(UpdateLow(0.5f));
    }

    /// <summary>
    /// 点击返回地图btn
    /// </summary>
    private void OnLowBackBtnClick()
    {
        SfxManager.Instance.PlaySfx("stage_cameraBack");
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
    /// 重置View
    /// </summary>
    private void ResetView()
    {
        StartCoroutine(UpdateHigh(1f));
        Color originalColor = lockStageItemClickText.color;
        originalColor.a = 0;
        lockStageItemClickText.color = originalColor;
    }

    /// <summary>
    /// 刷新地图
    /// </summary>
    /// <param name="during"></param>
    /// <returns></returns>
    private IEnumerator UpdateHigh(float during)
    {
        _fsm.ChangeFsmState(typeof(HighChangingState));
        yield return CameraManager.Instance.MoveObjCamera(_model.GetHighCamera(), during);
        //Debug.Log(cameraParams.Item1 + "  " + cameraParams.Item2 + "  " + cameraParams.Item3);
        _fsm.ChangeFsmState(typeof(HighSelectState));
    }
    
    private IEnumerator UpdateMid(float during)
    {
        yield return CameraManager.Instance.MoveObjCamera(_model.GetMidCamera(), during);
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
        yield return CameraManager.Instance.MoveObjCamera(_model.GetLowCamera(), during);
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