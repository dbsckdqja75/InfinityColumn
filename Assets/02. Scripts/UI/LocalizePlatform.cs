using UnityEngine;

[RequireComponent(typeof(LocalizeText))]
public class LocalizePlatform : MonoBehaviour
{
    #if UNITY_EDITOR || UNITY_STANDALONE
    [SerializeField] string stringKeyDesktop;

    LocalizeText localizeText;

    void Awake()
    {
        localizeText = this.GetComponent<LocalizeText>();
    }

    void Start()
    {
        localizeText.SetLocaleString(stringKeyDesktop);
    }
    #else
    void Awake()
    {
        Destroy(this.GetComponent<LocalizePlatform>());
    }
    #endif
}
