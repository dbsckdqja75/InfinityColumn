using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] LocalizeText gameModeText;

    public void UpdateGameModeText(string key)
    {
        gameModeText.SetLocaleString(key);
    }
}
