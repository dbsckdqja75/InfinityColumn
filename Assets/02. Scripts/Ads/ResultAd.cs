using UnityEngine;

public class ResultAd : MonoBehaviour
{
    [SerializeField] int exposureCount = 3;
    [SerializeField] AdButton targetAdButton;

    int requestCount = 0;

    void Awake()
    {
        requestCount = 0;
    }

    public void RequestAd()
    {
        requestCount++;

        if(requestCount >= exposureCount)
        {
            requestCount = 0;

            AdvertisementManager.Instance.RequestAd(targetAdButton);
        }
        else
        {
            targetAdButton.SetInteractable(false);
        }
    }
}
