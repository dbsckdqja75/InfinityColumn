using UnityEngine;
using UnityEngine.UI;

public class AdButton : MonoBehaviour
{
    [SerializeField] AdvertID advertID;

    [SerializeField] Button adButton;

    void Awake()
    {
        adButton.onClick.RemoveAllListeners();
        adButton.onClick.AddListener(() => { 
            AdvertisementManager.Instance.ShowAd(advertID, (amount) => 
            {
                CurrencyManager.Instance.RewardCurrency(CurrencyType.VOXEL_POINT, amount);

                Debug.LogFormat("[Mediation] OnReward ({0} : {1})", advertID, amount);
            });
        });
    }

    public void SetInteractable(bool isOn)
    {
        adButton.interactable = isOn;
    }
}
