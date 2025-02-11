using UnityEngine;
using TMPro;

public class VersionText : MonoBehaviour
{
    [SerializeField] TMP_Text versionText;

    void Awake()
    {
        versionText.text = string.Format("VER.{0}", Application.version);
    }
}
