using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class AdButton : MonoBehaviour
{
    [SerializeField] AdvertID advertID;

    [SerializeField] Button adButton;

    [Space(10)]
    [SerializeField] TMP_Text rewardText;
    [SerializeField] string rewardFormat = "+VP {0}";
    
    [SerializeField] UnityEvent enabledEvent, disabledEvent;
    [SerializeField] UnityEvent watchedEvent;

    void Awake()
    {
        adButton.onClick.AddListener(() => { 
            string title = LocalizationManager.Instance.GetString("WatchRewardedAd");
            string localeString = LocalizationManager.Instance.GetString("WatchRewardedAd_Context");
            string context = string.Format(localeString, AdvertisementManager.Instance.GetRewardAmount(advertID).ToString());
            ConfirmPopup.Instance.ConfirmFormat(title, context, () => { 
                AdvertisementManager.Instance.ShowAd(advertID, (amount) => 
                {
                    CurrencyManager.Instance.RewardCurrency(CurrencyType.VOXEL_POINT, amount);

                    watchedEvent?.Invoke();

                    rewardText.text = "WATCHED";

                    Debug.LogFormat("[Mediation] OnReward ({0} : {1})", advertID, amount);
                });
            });
        });
    }

    void OnEnable()
    {
        if(rewardText)
        {
            rewardText.text = string.Format(rewardFormat, AdvertisementManager.Instance.GetRewardAmount(advertID));
        }
    }

    public void OnWatched()
    {
        adButton.interactable = false;
    }

    public void SetInteractable(bool isOn)
    {
        if(isOn)
        {
            enabledEvent?.Invoke();
        }
        else
        {
            disabledEvent?.Invoke();
        }

        adButton.interactable = isOn;
    }
}
