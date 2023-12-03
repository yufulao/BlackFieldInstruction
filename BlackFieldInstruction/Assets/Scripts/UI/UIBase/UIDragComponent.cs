using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIDragComponent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public float validDragDistance=1f;//有效拖动的最小滑动距离
    public float validDragAngleForHorizontal=30f;//与横轴正方向的角度大于时才判定有效

    private RectTransform _dragRectTransform;
    private Graphic _graphic;
    private CanvasGroup _canvasGroup;

    private Func<GameObject, bool> _filter;
    private Action<List<GameObject>> _onEndDrag;
    private Action _onBeginDrag;
    private Action<PointerEventData> _invalidDragDispatch;//无效拖拽时转发的事件

    private GameObject _cacheObjOnDrag; //移动过程中复制的一份物体
    private Vector2 _originalPos; //开始拖拽的position


    public void InitDragComponent(Transform onDragParent, Func<GameObject, bool> filter, Action onBeginDrag = null,
        Action<List<GameObject>> onEndDrag = null,Action<PointerEventData> invalidDragDispatch=null)
    {
        _filter = filter;
        _onBeginDrag = onBeginDrag;
        _onEndDrag = onEndDrag;
        _invalidDragDispatch = invalidDragDispatch;
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        _graphic = GetComponent<Graphic>();
        _cacheObjOnDrag = Instantiate(this.gameObject, onDragParent);
        _cacheObjOnDrag.SetActive(false);
        _dragRectTransform = _cacheObjOnDrag.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_graphic)
        {
            _graphic.raycastTarget = false;
        }
        
        _originalPos = eventData.position;
        
        _onBeginDrag?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //最小输入
        if (Vector2.Distance(eventData.position, _originalPos) < validDragDistance)
        {
            return;
        }
        
        // //偏移向量
        // Vector2 direction = eventData.position - _originalPos;
        // //与X轴夹角
        // float angle = Mathf.Atan2(Mathf.Abs(direction.y), Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
        // //Debug.Log(angle );
        // if (angle < validDragAngleForHorizontal)
        // {
        //     // Debug.Log(angle);
        //     //_invalidDragDispatch?.Invoke(eventData);
        //     //CommandViewCtrl.Instance.WaitingCommandBtnInvalidDrag(eventData);
        //     //OnEndDrag(eventData);
        //     //return;
        // }
        GameObject.Find("CanvasTest").transform.Find("Scroll View").GetComponent<ScrollRect>().OnDrag(eventData);
        //CommandViewCtrl.Instance._view.waitingCommandScroll.OnDrag(eventData);
        return;

        //onDragBegin
        if (!_cacheObjOnDrag.activeInHierarchy)
        {
            _cacheObjOnDrag.SetActive(true);
            _canvasGroup.alpha = 0f;
        }

        _dragRectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_graphic)
        {
            _graphic.raycastTarget = true;
        }

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = eventData.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        _cacheObjOnDrag.SetActive(false);
        _canvasGroup.alpha = 1f;

        _onEndDrag?.Invoke(Filtrate(results));
    }

    private List<GameObject> Filtrate(List<RaycastResult> results)
    {
        // for (int i = 0; i < results.Count; i++)
        // {
        //     Debug.Log(results[i].gameObject.transform.name);
        // }

        for (int i = 0; i < results.Count; i++)
        {
            if (!_filter(results[i].gameObject))
            {
                results.Remove(results[i]);
            }
        }

        //移除当前拖拽的UI
        results.RemoveAll(result => result.gameObject == gameObject);
        ////移除当前拖拽的UI的所有子ui
        results.RemoveAll(result => IsChildOf(result.gameObject, gameObject));

        List<GameObject> resultObjs = new List<GameObject>();
        for (int i = 0; i < results.Count; i++)
        {
            resultObjs.Add(results[i].gameObject);
        }

        return resultObjs;
    }

    /// <summary>
    /// 检查子物体
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private bool IsChildOf(GameObject child, GameObject parent)
    {
        Transform t = child.transform;
        while (t != null)
        {
            if (t.gameObject == parent)
                return true;
            t = t.parent;
        }

        return false;
    }
}