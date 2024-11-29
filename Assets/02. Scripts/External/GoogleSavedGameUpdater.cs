using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleSavedGameUpdater : MonoBehaviour
{
    [SerializeField] CurrencyManager currencyManager;
    [SerializeField] CharacterManager characterManager;
    [SerializeField] PurchaseManager purchaseManager;

    public void OnLoadedGameData()
    {
        currencyManager.UpdateCurrencyData();
        characterManager.UpdateCharacterData();

        // TODO : 클라우드 데이터를 토대로 구매 상품 복원
    }
}
