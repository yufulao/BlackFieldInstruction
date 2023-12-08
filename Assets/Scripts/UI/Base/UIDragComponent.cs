using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIDragComponent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public float validDragDistance = 1f; //有效拖动的最小滑动距离
    public float validDragAngleForHorizontal = 30f; //与横轴正方向的角度大于时才判定有效

    private RectTransform _dragRectTransform;
    private Graphic _graphic;
    private CanvasGroup _canvasGroup;

    private Func<GameObject, bool> _filter;
    private Action<List<GameObject>,bool> _onEndDrag;
    private Action<PointerEventData> _onBeginDrag;
    private Action<PointerEventData> _invalidBeginDragDispatch; //无效拖拽开始时转发的事件
    private Action<PointerEventData> _invalidOnDragDispatch; //无效拖拽中转发的事件
    private Action<PointerEventData> _invalidEndDragDispatch; //无效拖拽结束时转发的事件

    private GameObject _cacheObjOnDrag; //移动过程中复制的一份物体
    private Vector2 _originalPos; //开始拖拽的position
    private bool _isValidDragging; //当前dragging是否是有效拖拽
    private bool _hadSetValidDrag; //是否已经设置了有效拖拽


    public void InitDragComponent(Transform onDragParent)
    {
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        _graphic = GetComponent<Graphic>();
        _cacheObjOnDrag = Instantiate(this.gameObject, onDragParent);
        _cacheObjOnDrag.SetActive(false);
        _dragRectTransform = _cacheObjOnDrag.GetComponent<RectTransform>();
    }

    public void SetDragAction(Func<GameObject, bool> filter, Action<PointerEventData> onBeginDrag = null, Action<List<GameObject>,bool> onEndDrag = null)
    {
        _filter = filter;
        _onBeginDrag = onBeginDrag;
        _onEndDrag = onEndDrag;
    }

    public void SetInvalidDragAction(Action<PointerEventData> invalidBeginDragDispatch = null, Action<PointerEventData> invalidOnDragDispatch = null,
        Action<PointerEventData> invalidEndDragDispatch = null)
    {
        _invalidBeginDragDispatch = invalidBeginDragDispatch;
        _invalidOnDragDispatch = invalidOnDragDispatch;
        _invalidEndDragDispatch = invalidEndDragDispatch;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_graphic)
        {
            _graphic.raycastTarget = false;
        }

        _originalPos = eventData.position;
        _isValidDragging = true;
        _hadSetValidDrag = false;

        _onBeginDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //最小输入
        if (!_hadSetValidDrag)
        {
            if (Vector2.Distance(eventData.position, _originalPos) > validDragDistance)
            {
                SetValidDragging(eventData);
                _hadSetValidDrag = true;
            }

            return;
        }

        if (!_isValidDragging)
        {
            _invalidOnDragDispatch?.Invoke(eventData);
            return;
        }

        //isValidDrag
        if (!_cacheObjOnDrag.activeInHierarchy)
        {
            _cacheObjOnDrag.SetActive(true);
            _canvasGroup.alpha = 0f;
        }

        _dragRectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isValidDragging)
        {
            _invalidEndDragDispatch?.Invoke(eventData);
            return;
        }

        //正常的valid的endDrag，相对的是OnChangeInvalidDrag
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

        _onEndDrag?.Invoke(Filtrate(results),true);//true指的是，是validDrag
    }

    private void SetValidDragging(PointerEventData eventData)
    {
        //偏移向量
        Vector2 direction = eventData.position - _originalPos;
        //与X轴夹角
        float angle = Mathf.Atan2(Mathf.Abs(direction.y), Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
        //Debug.Log(angle );
        if (angle < validDragAngleForHorizontal)
        {
            _invalidBeginDragDispatch?.Invoke(eventData);
            OnChangeInvalidDrag(eventData);//切换为InvalidDrag的时候执行
            _isValidDragging = false;
        }
    }

    /// <summary>
    /// 切换为invalidDrag的时候需要做的事，执行了这个就不会执行正常的validOnEndDrag
    /// </summary>
    /// <param name="eventData"></param>
    private void OnChangeInvalidDrag(PointerEventData eventData)
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

        _onEndDrag?.Invoke(Filtrate(results),false);//false指的是，不是validDrag
    }

    private List<GameObject> Filtrate(List<RaycastResult> results)
    {
        for (int i = 0; i < results.Count; i++)
        {
            if (!_filter(results[i].gameObject))
            {
                results.Remove(results[i]);
            }
        }

        //移除当前拖拽的UI
        results.RemoveAll(result => result.gameObject == gameObject);
        //移除当前拖拽的UI的所有子ui
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