﻿using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField] bool onResizeSafeArea;

    Vector2 minAnchor, maxAnchor;

    void Start()
    {
        if(onResizeSafeArea)
        {
            ResizeSafeArea();
        }
    }

    void ResizeSafeArea()
    {
        RectTransform panelRect = this.GetComponent<RectTransform>();

        minAnchor = Screen.safeArea.min;
        maxAnchor = Screen.safeArea.max;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;

        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        panelRect.anchorMin = minAnchor;
        panelRect.anchorMax = maxAnchor;
    }

    public void SetPanel(bool isOn)
    {
        this.gameObject.SetActive(isOn);
    }

    public virtual void Open()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        this.gameObject.SetActive(false);
    }
}
