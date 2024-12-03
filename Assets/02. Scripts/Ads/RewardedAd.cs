using System;
using com.unity3d.mediation;
using UnityEngine;

using TMPro;

public class RewardedAd : MonoBehaviour
{
    string currentPlacement;

    Action<int> rewardCallback;

    void Start()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
    }

    public void ShowRewardedVideoAd(string placementName, Action<int> callback)
    {
        Debug.Log ("[Mediation] OnShowRewardedVideo");

        if(IronSource.Agent.isRewardedVideoAvailable())
        {
            currentPlacement = placementName;
            rewardCallback = callback;

            IronSource.Agent.showRewardedVideo();

            // TODO : 실제 적용 시에는 placementName 적용
            // IronSource.Agent.showRewardedVideo(placementName);
        }
    }

    public void LoadRewardedAd()
    {
        if(IronSource.Agent.isRewardedVideoAvailable() == false)
        {
            IronSource.Agent.loadRewardedVideo();
        }
    }

    public bool IsAdReady()
    {
        return IronSource.Agent.isRewardedVideoAvailable();
    }
    
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got RewardedVideoOnAdOpenedEvent With AdInfo " + adInfo);
	}

	void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
	{
        IronSourcePlacement placement = IronSource.Agent.getPlacementInfo(currentPlacement);
        Debug.Log("[Mediation] Placement Reward Amount : " + placement.getRewardAmount());

        // TODO : DEBUG TEXT
        if(GameObject.Find("DEBUG_ADS"))
        {
            GameObject.Find("DEBUG_ADS").GetComponent<TMP_Text>().text = string.Format("RewardedVideoOnAdClosedEvent\nReward Count : {0}", placement.getRewardAmount());
        }

        currentPlacement = "";

        rewardCallback?.Invoke(placement.getRewardAmount());
        rewardCallback = null;

		Debug.Log("[Mediation]  I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo);
	}

	void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got RewardedVideoOnAdAvailable With AdInfo " + adInfo);
	}

	void RewardedVideoOnAdUnavailable()
	{
		Debug.Log("[Mediation] I got RewardedVideoOnAdUnavailable");
	}

	void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got RewardedVideoAdOpenedEvent With Error" + ironSourceError + "And AdInfo " + adInfo);
	}

	void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
	}

	void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got RewardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
	}
}
