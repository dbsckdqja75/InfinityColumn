using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class InputController : MonoBehaviour
{
    bool isPointerObject;

    UnityEvent<Vector3> onTouchEvent = new UnityEvent<Vector3>();

    Vector2 inputPosition;

    public void UpdateMousePosition(InputAction.CallbackContext context)
    {
        inputPosition = context.ReadValue<Vector2>();
    }

    public void OnTouch(InputAction.CallbackContext context)
    {
        TouchState touchState = context.ReadValue<TouchState>();
        if(touchState.phase == TouchPhase.Began)
        {
            inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            OnTouchEvent(inputPosition);
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton())
        {
            OnTouchEvent(inputPosition);
        }
    }

    void OnTouchEvent(Vector3 iPosition)
    {
        if (IsPointerOverUIObject())
            return;

        onTouchEvent?.Invoke(iPosition);
    }

    public void AddTouchEvent(UnityAction<Vector3> action)
    {
        onTouchEvent.AddListener(action);
    }

    public Vector2 GetPointerPosition()
    {
        return inputPosition;
    }

    int GetPointerRayCount()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = new Vector2(inputPosition.x, inputPosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count;
    }

    bool IsPointerOverUIObject(bool isEndInput = false)
    {
        isPointerObject = false;

        if (GetPointerRayCount() > 0)
        {
            if (isEndInput)
                return true;

            isPointerObject = true;
        }

        return isPointerObject;
    }
}
