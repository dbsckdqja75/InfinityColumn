using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class ButtonAnimation : MonoBehaviour
{
    Button targetButton;

    Animator animator;

    EventTrigger eventTrigger;

    void Awake()
    {
        animator = this.GetComponent<Animator>();

        if(this.TryGetComponent<Button>(out targetButton))
        {
            eventTrigger = this.GetComponent<EventTrigger>();
        }
        else
        {
            targetButton = this.GetComponentInChildren<Button>();
            eventTrigger = this.GetComponentInChildren<EventTrigger>();
        }

        if(eventTrigger != null)
        {
            EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
            entry_PointerDown.eventID = EventTriggerType.PointerDown;
            entry_PointerDown.callback.AddListener((data) => { OnPressed(); });
            eventTrigger.triggers.Add(entry_PointerDown);

            EventTrigger.Entry entry_PointerUp = new EventTrigger.Entry();
            entry_PointerUp.eventID = EventTriggerType.PointerUp;
            entry_PointerUp.callback.AddListener((data) => { OnReset(); });
            eventTrigger.triggers.Add(entry_PointerUp);

            // EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
            // entry_PointerExit.eventID = EventTriggerType.PointerExit;
            // entry_PointerExit.callback.AddListener((data) => { OnReset(); });
            // eventTrigger.triggers.Add(entry_PointerExit);

            EventTrigger.Entry entry_Deselect = new EventTrigger.Entry();
            entry_Deselect.eventID = EventTriggerType.Deselect;
            entry_Deselect.callback.AddListener((data) => { OnReset(); });
            eventTrigger.triggers.Add(entry_Deselect);
        }
    }

    void OnPressed()
    {
        if(targetButton.interactable)
        {
            animator.SetTrigger("Pressed");
        }
    }

    void OnReset()
    {
        animator.SetTrigger("Normal");
    }
}
