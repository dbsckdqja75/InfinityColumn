using UnityEngine;

public class GoogleSavedGameUpdater : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] CurrencyManager currencyManager;
    [SerializeField] CharacterManager characterManager;

    public void OnLoadedGameData()
    {
        gameManager.UpdateBestScore();
        currencyManager.UpdateCurrencyData();
        characterManager.UpdateCharacterData();
    }

    #if !UNITY_ANDROID
    void OnEnable()
    {
        Destroy(this.gameObject);
    }
    #endif
}
