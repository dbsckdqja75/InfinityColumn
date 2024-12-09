using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardedProduct : MonoBehaviour
{
    bool canPurchase = false;

    [SerializeField] Image mainBackground;
    [SerializeField] Image[] extraBackground;
    [SerializeField] Color enableColor, disableColor;

    [Space(10)]
    [SerializeField] GameObject rewardLabelObj;

    [Space(10)]
    [SerializeField] AdButton targetAdButton;

    void Awake()
    {
        canPurchase = false;

        targetAdButton.SetInteractable(false);
    }

    void OnEnable()
    {
        this.StopAllCoroutines();

        DateCheck().Start(this);
    }

    void UpdateDate()
    {
        DateTime lastDateTime = PlayerPrefsManager.LoadData("LastDailyRewardedDate", DateManager.Instance.todayDateTime.AddDays(-1));
        if(DateManager.Instance.IsDateEqual(lastDateTime) == false)
        {
            canPurchase = true;

            if(AdvertisementManager.Instance != null)
            {
                AdvertisementManager.Instance.RequestAd(targetAdButton);
            }
        }
    }

    public void OnPurchased()
    {
        canPurchase = false;

        targetAdButton.SetInteractable(false);

        OnDeactivate();

        PlayerPrefsManager.SaveData("LastDailyRewardedDate", DateManager.Instance.todayDateTime);
    }

    public void OnActive()
    {
        SetActiveBackground(canPurchase);

        rewardLabelObj.SetActive(canPurchase);
    }

    public void OnDeactivate()
    {
        SetActiveBackground(false);

        rewardLabelObj.SetActive(false);
    }

    void SetActiveBackground(bool isOn)
    {
        mainBackground.color = isOn ? enableColor : disableColor;

        foreach(Image background in extraBackground)
        {
            background.enabled = isOn;
        }
    }

    IEnumerator DateCheck()
    {
        int tryCount = 3;

        while(DateManager.Instance.IsSynced() == false)
        {
            if(tryCount <= 0)
            {
                yield break;
            }

            yield return new WaitForSeconds(3f);
            yield return new WaitForEndOfFrame();

            tryCount--;
        }

        if(DateManager.Instance.IsSynced())
        {
            UpdateDate();
        }

        yield break;
    }
}
