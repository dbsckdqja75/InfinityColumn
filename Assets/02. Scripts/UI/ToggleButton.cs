using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] Sprite enabledSprite;
    [SerializeField] Sprite disabledSprite;
    [SerializeField] Image targetImage;

    public void SetToggle(bool isEnable)
    {
        targetImage.sprite = isEnable ? enabledSprite : disabledSprite;
    }
}
