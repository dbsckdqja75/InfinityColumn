using UnityEngine;

public class GoogleSavedGameUpdater : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] CurrencyManager currencyManager;
    [SerializeField] CharacterManager characterManager;
    [SerializeField] PurchaseManager purchaseManager;

    public void OnLoadedGameData()
    {
        gameManager.UpdateBestScoreData();
        currencyManager.UpdateCurrencyData();
        characterManager.UpdateCharacterData();

        // TODO : 클라우드에 저장된 데이터를 토대로 구매 상품 복원 처리
    }

    #if !UNITY_ANDROID
    void Awake()
    {
        Destroy(this.gameObject);
    }
    #endif
}
