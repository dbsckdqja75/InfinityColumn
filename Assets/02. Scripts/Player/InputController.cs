using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{

    bool isPointerObject;

    [HideInInspector] public UnityEvent<Vector3> onTouchEvent = new UnityEvent<Vector3>();

    void Update()
    {
    #if !UNITY_EDITOR && !UNITY_STANDALONE
        UpdateTouch();
    #else
        UpdateInput();
    #endif
    }

    // NOTE : 테스트용
    void UpdateInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnTouchEvent(Input.mousePosition);
        }
    }

    // NOTE : 모바일 기기 터치 구현부
    #if !UNITY_EDITOR && !UNITY_STANDALONE
    void UpdateTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // TODO : 실제 안드로이드 빌드해서 테스트 확인 필요 (touch.position 동작 여부 확인)
                    OnTouchEvent(Input.mousePosition);
                    // OnTouchEvent(touch.position);
                    break;
                default:
                    break;
            }
        }
    }
    #endif

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

    int GetPointerRayCount()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

    #if !UNITY_EDITOR && !UNITY_STANDALONE
        eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
    #else
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    #endif

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
