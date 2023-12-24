using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CameraManager : BaseSingleTon<CameraManager>, IMonoManager
{
    private Transform _cameraContainer;
    private Camera _objCamera;
    private Camera _uiCamera;
    private Sequence _objSequence;

    private Camera _cacheMainObjCamera;


    public void OnInit()
    {
        _cameraContainer = GameObject.Find("CameraContainer").transform;
        _objCamera = _cameraContainer.Find("ObjCamera").GetComponent<Camera>();
        _cacheMainObjCamera = _objCamera;
        _uiCamera = _cameraContainer.Find("UICamera").GetComponent<Camera>();
        EventManager.Instance.AddListener(EventName.ChangeScene,OnChangeScene);
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void OnClear()
    {
        _objSequence?.Kill();
    }

    public IEnumerator MoveObjCamera(Vector3 targetPosition,Vector3 targetRotation, float fieldOfView, float during = 0f)
    {
        _objSequence?.Kill();
        _objSequence = DOTween.Sequence();
        _objSequence.Join(_objCamera.transform.DOLocalMove(targetPosition, during));
        _objSequence.Join(_objCamera.transform.DOLocalRotateQuaternion(Quaternion.Euler(targetRotation), during));
        _objSequence.Join(_objCamera.DOFieldOfView(fieldOfView, during));
        _objSequence.SetAutoKill(false);
        yield return _objSequence.WaitForCompletion();
    }

    public Camera GetObjCamera()
    {
        return _objCamera;
    }

    private void OnChangeScene()
    {
        _objSequence?.Kill();
        GameObject sceneObjCameraObj = GameObject.Find("Main Camera");
        if (!sceneObjCameraObj)
        {
            return;
        }
        
        Camera sceneObjCamera = sceneObjCameraObj.GetComponent<Camera>() ?? null;
        
        if (sceneObjCamera)
        {
            _objCamera.gameObject.SetActive(false);
            _objCamera = sceneObjCamera;
            return;
        }

        if (!_objCamera.gameObject.activeInHierarchy)
        {
            _objCamera = _cacheMainObjCamera;
            _objCamera.gameObject.SetActive(true);
        }
    }
}