using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : BaseSingleTon<InputManager>,IMonoManager
{
    private readonly InputActions _inputActions=new InputActions();
    
    public void OnInit()
    {
        _inputActions.Enable();
        //UI是映射列表, Click是一个InputAction名字
        _inputActions.UI.Click.performed += OnClick;
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

    private void OnClick(InputAction.CallbackContext callbackContext)
    {
        EventManager.Instance.Dispatch(EventName.Click);
    }
}
