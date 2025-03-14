using System;
using Unity.Services.LevelPlay;
using UnityEngine;

using TMPro;

public class InterstitialAd : MonoBehaviour
{
    #if UNITY_ANDROID
    string interstitialAdUnitId = "aeyqi3vqlv6o8sh9";
    #elif UNITY_IPHONE
    string interstitialAdUnitId = "wmgt0712uuux8ju4";
    #else
    string interstitialAdUnitId = "unexpected_platform";
    #endif

    string currentPlacement;

    LevelPlayInterstitialAd interstitialAd;

    Action<int> rewardCallback;

    public void ShowInterstitialAd(string placementName, Action<int> callback)
    {
        Debug.Log ("[Mediation] ShowInterstitialAd");

        if(interstitialAd != null)
        {
            if(interstitialAd.IsAdReady())
            {
                currentPlacement = placementName;
                rewardCallback = callback;

                interstitialAd.ShowAd();
                
                // TODO : 실제 적용 시에는 placementName 적용
                // interstitialAd.ShowAd(placementName);
            }
        }
    }

    public void LoadInterstitialAd()
    {
        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;

        interstitialAd.LoadAd();
    }

    public bool IsAdReady()
    {
        if(interstitialAd != null)
        {
            return interstitialAd.IsAdReady();
        }
        
        return false;
    }

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got InterstitialOnAdLoadedEvent With AdInfo " + adInfo);
	}

	void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
	{
		Debug.Log("[Mediation] I got InterstitialOnAdLoadFailedEvent With Error " + error);
	}
	
	void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got InterstitialOnAdDisplayedEvent With AdInfo " + adInfo);
	}
	
	void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError)
	{
		Debug.Log("[Mediation] I got InterstitialOnAdDisplayFailedEvent With InfoError " + infoError);
	}
	
	void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got InterstitialOnAdClickedEvent With AdInfo " + adInfo);
	}

	void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
	{
        IronSourcePlacement placement = IronSource.Agent.getPlacementInfo(currentPlacement);
        Debug.Log("[Mediation] Placement Reward Amount : " + placement.getRewardAmount());

        // TODO : DEBUG TEXT
        if(GameObject.Find("DEBUG_ADS"))
        {
            GameObject.Find("DEBUG_ADS").GetComponent<TMP_Text>().text = string.Format("InterstitialOnAdClosedEvent\nReward Count : {0}", placement.getRewardAmount());
        }

        currentPlacement = "";
        
        rewardCallback?.Invoke(placement.getRewardAmount());
        rewardCallback = null;

		Debug.Log("[Mediation] I got InterstitialOnAdClosedEvent With AdInfo " + adInfo);
	}
	
	void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
	{
		Debug.Log("[Mediation] I got InterstitialOnAdInfoChangedEvent With AdInfo " + adInfo);
	}
	
	private void OnDestroy()
	{
        if(interstitialAd != null)
        {
		    interstitialAd.DestroyAd();
        }
	}
}
