using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class LocalizationManager : MonoSingleton<LocalizationManager>
{

    [SerializeField] TableReference tableReference = "InGameLocalization";

    Coroutine changeLocaleCoroutine;

    public int SwitchLanguage(int language)
    {
        int languageCount = LocalizationSettings.AvailableLocales.Locales.Count;
        language = (int)Mathf.Repeat(language+1, languageCount);

        ChangeLanguage(language);

        return language;
    }

    public void ChangeLanguage(int language)
    {
        changeLocaleCoroutine?.Stop(this);
        changeLocaleCoroutine = ChangeLocale(language).Start(this);
    }

    public string GetString(string localeKey)
    {
        Locale currentLanguage = LocalizationSettings.SelectedLocale;
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableReference, localeKey, currentLanguage);
    }

    IEnumerator ChangeLocale(int index)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        yield break;
    }

    public int GetDefaultLanguage()
    {
        switch(Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                return 1;
            case SystemLanguage.Japanese:
                return 2;
            default: 
                return 0; // NOTE : SystemLanguage.English
        }
    }
}