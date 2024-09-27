using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PanelWithAnimation : Panel
{
    [SerializeField] UnityEvent startEvent;
    [SerializeField] UnityEvent endEvent;

    Animator animator;

    Coroutine motion;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    void OnEnable() 
    {
        Open();    
    }

    void OnDisable() 
    {
        endEvent?.Invoke();
    }

    public override void Open()
    {
        MotionClear();

        motion = PlayMotion("FadeIn").Start(this);
    }

    public override void Close()
    {
        MotionClear();

        motion = PlayMotion("FadeOut", () => { base.Close(); }).Start(this);
    }

    void MotionClear()
    {
        if(motion != null)
        {
            motion.Stop(this);
        }
    }

    IEnumerator PlayMotion(string trigger, Action endCallback = null)
    {
        startEvent?.Invoke();

        animator.SetTrigger(trigger);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(trigger));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        endEvent?.Invoke();
        endCallback?.Invoke();

        yield break;
    }
}
