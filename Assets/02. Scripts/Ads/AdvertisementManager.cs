using System;
using com.unity3d.mediation;
using UnityEngine;
using TMPro;

public class AdvertisementManager : MonoSingleton<AdvertisementManager>
{
    bool isInitialized = false;

    // NOTE : 테스트 데모용 Key
    #if UNITY_ANDROID
	const string appKey = "85460dcd";
    #elif UNITY_IPHONE
    const string appKey = "8545d445";
    #else
    const string appKey = "unexpected_platform";
    #endif

    #if UNITY_ANDROID
    string[] adPlacements = { "GameResult_Rewarded_Android ", "GameResult_Rewarded_Android", "Daily_Rewarded_Android" };
    #elif UNITY_IPHONE
    string[] adPlacements = { "GameResult_Rewarded_iOS ", "GameResult_Rewarded_iOS", "Daily_Rewarded_iOS" };
    #else
    string[] adPlacements = { "unexpected_platform ", "unexpected_platform", "unexpected_platform" };
    #endif

    [SerializeField] RewardedAd rewardedAd;
    [SerializeField] InterstitialAd interstitialAd;

    protected override void Init()
    {
        // TODO : 추후에 약관동의를 통해 여부 반영
        IronSource.Agent.setConsent(true);
        IronSource.Agent.setMetaData("do_not_sell","true");

        IronSourceConfig.Instance.setClientSideCallbacks (true);

        string userID = SystemInfo.deviceUniqueIdentifier;
        Debug.LogFormat("[Mediation] IronSource Local UserID : {0}", userID);

        string advertiserId= IronSource.Agent.getAdvertiserId();
        Debug.LogFormat("[Mediation] IronSource.Agent.getAdvertiserId : {0}", advertiserId);

        IronSource.Agent.validateIntegration ();
        Debug.Log("[Mediation] IronSource.Agent.validateIntegration");

        Debug.LogFormat("[Mediation] IronSource Unity Version : {0}", IronSource.unityVersion());

        Debug.Log("[Mediation] LevelPlay Init");
		LevelPlay.Init(appKey, userID , new [] { LevelPlayAdFormat.INTERSTITIAL, LevelPlayAdFormat.REWARDED });
		
		LevelPlay.OnInitSuccess += OnInitializationCompleted;
		LevelPlay.OnInitFailed += (error => Debug.LogFormat("[Mediation] Initialization error : {0}", error));
    }

	void OnInitializationCompleted(LevelPlayConfiguration configuration)
	{
        isInitialized = true;

        interstitialAd.LoadInterstitialAd();

		Debug.Log("[Mediation] Initialization completed");
	}

    public bool IsReadyAd()
    {
        if(isInitialized)
        {
            return (rewardedAd.IsAdReady() || interstitialAd.IsAdReady());
        }

        return false;
    }

    // TODO : 버튼의 액션으로만 적용
    public void ShowAd(AdvertID advertID, Action<int> rewardCallback)
    {
        #if UNITY_EDITOR
        rewardCallback?.Invoke(GetRewardAmount(advertID));
        return;
        #endif

        // TODO : 광고 제거 상품 보유 시 별도 처리
        if(rewardedAd.IsAdReady())
        {
            rewardedAd.ShowRewardedVideoAd(GetPlacementName(advertID), rewardCallback);
            return;
        }

        if(interstitialAd.IsAdReady())
        {
            interstitialAd.ShowInterstitialAd(GetPlacementName(advertID), rewardCallback);
            return;
        }
    }

    // TODO : 실제로 광고 버튼 노출 요청 시, RequestAd로 호출
    public void RequestAd(AdButton adButton)
    {
        adButton.SetInteractable(false);

        #if UNITY_EDITOR
            adButton.SetInteractable(true);
        #else
        if(isInitialized)
        {
            // TODO : DEBUG TEXT
            if(GameObject.Find("DEBUG_ADS"))
            {
                GameObject.Find("DEBUG_ADS").GetComponent<TMP_Text>().text = string.Format("IsReadyAd\n({0} / {1})", rewardedAd.IsAdReady(), interstitialAd.IsAdReady());
            }

            if(IsReadyAd() == false)
            {
                PrepareAd();
            }

            adButton.SetInteractable(IsReadyAd());
        }
        #endif
    }

    // TODO : Test Code
    public void Test(Action<int> rewardCallback)
    {
        if(interstitialAd.IsAdReady())
        {
            interstitialAd.ShowInterstitialAd("GameResult_Rewarded_Android", rewardCallback);
        }
    }

    void PrepareAd()
    {
        if(rewardedAd.IsAdReady() == false)
        {
            rewardedAd.LoadRewardedAd();
        }

        if(interstitialAd.IsAdReady() == false)
        {
            interstitialAd.LoadInterstitialAd();
        }
    }

    public int GetRewardAmount(AdvertID advertID)
    {
        if(isInitialized)
        {
            IronSourcePlacement placement = IronSource.Agent.getPlacementInfo(GetPlacementName(advertID));

            if(placement != null)
            {
                return placement.getRewardAmount();
            }
        }

        return 0;
    }

    string GetPlacementName(AdvertID advertID)
    {
        return adPlacements[(int)advertID];
    }
}