using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 事件管理器
/// </summary>
public class EventManager:Singleton<EventManager>,IMonoManager
{
    private readonly Dictionary<EventName, Action> _eventDic = new Dictionary<EventName, Action>();
    
    /// <summary>
    /// 添加一个事件的监听者
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="action">事件处理函数</param>
    public void AddListener(EventName eventName,Action action)
    {
        if (_eventDic.ContainsKey(eventName))
        {
            _eventDic[eventName] += action;
        }
        else
        {
            _eventDic.Add(eventName, action);
        }
    }
    
    /// <summary>
    /// 移除一个事件的监听者
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="action">事件处理函数</param>
    public void RemoveListener(EventName eventName, Action action)
    {
        if (_eventDic.ContainsKey(eventName))
        {
            _eventDic[eventName] -= action;
        }
    }
    
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    public void Dispatch(EventName eventName)
    {
        if (_eventDic.ContainsKey(eventName))
        {
            _eventDic[eventName]?.Invoke(); 
        }
    }

    /// <summary>
    /// 清空所有事件
    /// </summary>
    public void Clear()
    {
        _eventDic.Clear();
    }

    public void OnInit()
    {
        
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
        
    }
}