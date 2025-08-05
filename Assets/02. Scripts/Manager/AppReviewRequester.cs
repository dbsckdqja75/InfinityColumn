using UnityEngine;

#if UNITY_ANDROID
using Google.Play.Review;
using Review = Google.Play.Review.ReviewManager;
#endif

public class AppReviewRequester : MonoBehaviour
{
    [SerializeField] int exposureCount = 8;
    int requestCount = 0;

    #if UNITY_ANDROID
    Review review;

    void Start()
    {
        review = new Review();
    }
    #endif

    void Awake()
    {
        requestCount = 0;
    }

    public void RequestReview()
    {
        if(requestCount != exposureCount)
        {
            requestCount += 1;
            return;
        }

        #if UNITY_ANDROID
        var playReviewInfoAsyncOperation = review.RequestReviewFlow();

        playReviewInfoAsyncOperation.Completed += 
        playReviewInfoAsync => 
        {
            if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
            {
                var playReviewInfo = playReviewInfoAsync.GetResult();
                review.LaunchReviewFlow(playReviewInfo);    

                Debug.Log("NoError (LaunchReviewFlow)");
            }
            else
            {
                Debug.LogWarning(playReviewInfoAsync.Error.ToString());
            }

            Debug.Log("playReviewInfoAsyncOperation.Completed");
        };
        #else

        // TODO : iOS 리뷰 요청 처리
        
        #endif

        requestCount += 1;
    }
}
