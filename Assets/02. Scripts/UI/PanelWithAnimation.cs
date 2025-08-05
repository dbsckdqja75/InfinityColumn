using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PanelWithAnimation : Panel
{
    [SerializeField] RectTransform resizeTargetPanel;
    [SerializeField] UnityEvent startEvent;
    [SerializeField] UnityEvent endEvent;

    Animator animator;

    Coroutine motion;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    void Start()
    {
        if(onResizeSafeArea && resizeTargetPanel != null)
        {
            ResizeSafeArea(resizeTargetPanel);
        }
    }

    void OnEnable() 
    {
        Open();
    }

    void OnDisable() 
    {
        motion = null;

        endEvent?.Invoke();
    }

    public override void Open()
    {
        if(gameObject.activeSelf)
        {
            if(!IsMotionPlaying())
            {
                motion = PlayMotion("FadeIn").Start(this);
            }
        }
    }

    public override void Close()
    {
        if(gameObject.activeSelf)
        {
            if(!IsMotionPlaying())
            {
                motion = PlayMotion("FadeOut", () => { base.Close(); }).Start(this);
            }
            else
            {
                motion = null;
                base.Close();
            }
        }
    }

    bool IsMotionPlaying()
    {
        return (motion != null);
    }

    IEnumerator PlayMotion(string trigger, Action endCallback = null)
    {
        startEvent?.Invoke();

        animator.SetTrigger(trigger);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(trigger));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        endEvent?.Invoke();
        endCallback?.Invoke();

        motion = null;

        yield break;
    }
}
