using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    enum ButtonSoundType 
    {
        NORMAL,
        CANCEL,
    }

    [SerializeField] ButtonSoundType soundType = ButtonSoundType.NORMAL;

    void Awake()
    {
        string soundName = "";
        Button button = this.GetComponent<Button>();

        switch(soundType)
        {
            case ButtonSoundType.CANCEL:
            soundName = "Click2";
            break;
            default:
            soundName = "Click1";
            break;
        }

        button.onClick.AddListener(() => SoundManager.Instance.PlaySound(soundName) );
    }
}
