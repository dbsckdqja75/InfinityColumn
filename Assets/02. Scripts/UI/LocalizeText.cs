using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

[RequireComponent(typeof(TMP_Text), typeof(LocalizeStringEvent))]
public class LocalizeText : MonoBehaviour
{
    TMP_Text tmpText;
    LocalizeStringEvent localizeStringEvent;
    string currentKey = "", formatValue = "";

    void Awake()
    {
        tmpText = this.GetComponent<TMP_Text>();
        localizeStringEvent = this.GetComponent<LocalizeStringEvent>();
    }

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += UpdateLocale;

        UpdateLocale(LocalizationSettings.SelectedLocale);
    }

    void OnDisable() 
    {
        LocalizationSettings.SelectedLocaleChanged -= UpdateLocale;
    }

    public void SetLocaleString(string key)
    {
        currentKey = key;

        if(localizeStringEvent)
        {
            LocalizedString stringReference = localizeStringEvent.StringReference;
            stringReference.TableEntryReference = currentKey;

            localizeStringEvent.StringReference = stringReference;
            localizeStringEvent.RefreshString();
        }

        UpdateLocale(LocalizationSettings.SelectedLocale);
    }

    public void SetStringFormatValue(string context)
    {
        formatValue = context;

        UpdateLocale(LocalizationSettings.SelectedLocale);
    }

    void UpdateLocale(Locale locale)
    {
        if(tmpText && currentKey.Length > 0)
        {
            string localeText = LocalizationManager.Instance.GetString(currentKey);
            tmpText.text = (formatValue.Length > 0) ? string.Format(localeText, formatValue): localeText;
        }
    }
}
