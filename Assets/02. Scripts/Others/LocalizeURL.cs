using UnityEngine;

public class LocalizeURL : MonoBehaviour
{
    [SerializeField] string koreanURL;
    [SerializeField] string englishURL;

    public void OpenURL()
    {
        if(Application.systemLanguage == SystemLanguage.Korean)
        {
            Application.OpenURL(koreanURL);
        }
        else
        {
            Application.OpenURL(englishURL);
        }
    }
}
