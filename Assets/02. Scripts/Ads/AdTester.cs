using UnityEngine;

public class AdTester : MonoBehaviour
{
    [SerializeField] AdButton adButtonA, adButtonB, adButtonC;

    AdvertisementManager advertisement;

    void Start()
    {
        AllRequest();
    }

    public void AllRequest()
    {
        if(AdvertisementManager.Instance != null)
        {
            AdvertisementManager.Instance.RequestAd(adButtonA);
            AdvertisementManager.Instance.RequestAd(adButtonB);
            AdvertisementManager.Instance.RequestAd(adButtonC);
        }
    }

    public void TestRequest()
    {
        if(AdvertisementManager.Instance != null)
        {
            AdvertisementManager.Instance.Test((amount) => 
            {
                CurrencyManager.Instance.RewardCurrency(CurrencyType.VOXEL_POINT, amount);

                Debug.LogFormat("[Mediation] OnReward ({0} : {1})", AdvertID.LOBBY_REWARDED, amount);
            });
        }
    }
}
