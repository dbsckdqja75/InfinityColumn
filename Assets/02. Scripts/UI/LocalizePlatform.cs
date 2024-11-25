using UnityEngine;

[RequireComponent(typeof(LocalizeText))]
public class LocalizePlatform : MonoBehaviour
{
    [SerializeField] string stringKeyDesktop;

    #if UNITY_EDITOR || UNITY_STANDALONE
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
