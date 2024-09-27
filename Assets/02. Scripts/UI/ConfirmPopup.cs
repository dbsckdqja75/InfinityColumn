using System;
using UnityEngine;
using TMPro;

public class ConfirmPopup : MonoSingleton<ConfirmPopup>
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text confirmText;
    
    [SerializeField] TMP_Text yesText;
    [SerializeField] TMP_Text noText;

    [SerializeField] GameObject confirmPopup;

    Action confirmAction, cancelAction;

    public void Confirm(string titleKey, string contextKey, Action callback)
    {
        titleText.text = LocalizationManager.Instance.GetString(titleKey);
        confirmText.text = LocalizationManager.Instance.GetString(contextKey);

        yesText.text = LocalizationManager.Instance.GetString("Yes");
        noText.text = LocalizationManager.Instance.GetString("No");

        confirmAction = callback;
        cancelAction = null;

        confirmPopup.SetActive(true);
    }

    public void Confirm(string title, string context, string yes, string no, Action callback)
    {
        titleText.text = title;
        confirmText.text = context;

        yesText.text = yes;
        noText.text = no;

        confirmAction = callback;
        cancelAction = null;

        confirmPopup.SetActive(true);
    }

    public void Confirm(string title, string context, Action confirmCallback, Action cancelCallback)
    {
        titleText.text = title;
        confirmText.text = context;

        confirmAction = confirmCallback;
        cancelAction = cancelCallback;

        confirmPopup.SetActive(true);
    }

    public void Confirm(string title, string context, string yes, string no, Action confirmCallback, Action cancelCallback)
    {
        titleText.text = title;
        confirmText.text = context;

        yesText.text = yes;
        noText.text = no;

        confirmAction = confirmCallback;
        cancelAction = cancelCallback;

        confirmPopup.SetActive(true);
    }

    public void Yes()
    {
        confirmAction?.Invoke();

        confirmPopup.SetActive(false);
    }

    public void No()
    {
        cancelAction?.Invoke();
        
        confirmPopup.SetActive(false);
    }
}
