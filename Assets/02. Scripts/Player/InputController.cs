using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    bool isPointerObject;

    [HideInInspector] public UnityEvent<Vector3> onTouchEvent = new UnityEvent<Vector3>();

    Vector2 inputPosition;

    public void UpdateInputPosition(InputAction.CallbackContext context)
    {
        inputPosition = context.ReadValue<Vector2>();
    }

    public void OnTouch(InputAction.CallbackContext context)
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
