using UnityEngine;
using TMPro;

public class VersionText : MonoBehaviour
{
    [SerializeField] TMP_Text versionText;

    void Awake()
    {
        versionText.text = string.Format("공개 테스트 버전 ({0})", Application.version);
    }
}
