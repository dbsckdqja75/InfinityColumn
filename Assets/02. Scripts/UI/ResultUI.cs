using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    bool canSkipResult = false;

    [SerializeField] TMP_Text resultScoreText;
    [SerializeField] TMP_Text resultBestScoreText;
    [SerializeField] TMP_Text rewardVpText;
    [SerializeField] GameObject rewardBoxObj, rewardObj;
    [SerializeField] GameObject newRecordBoxObj, newRecordObj;

    [Space(10)]
    [SerializeField] Animation[] animList;

    [Space(10)]
    [SerializeField] ResultAd resultAd;

    public void ReportScore(int score, int bestScore)
    {
        resultScoreText.text = string.Format("{0:N0}", score.ToString());
        resultBestScoreText.text = string.Format("{0:N0}", bestScore.ToString());
    }

    public void ReportReward(int reward)
    {
        rewardVpText.text = string.Format("+VP {0}", reward);
        rewardObj.SetActive(true);
    }

    public void OnResult(bool isNewRecord)
    {
        if(isNewRecord)
        {
            newRecordObj.SetActive(true);
        }

        #if UNITY_ANDROID || UNITY_IPHONE
        resultAd.RequestAd();
        #endif
    }

    public void ResetUI()
    {
        canSkipResult = false;

        rewardBoxObj.SetActive(false);
        rewardObj.SetActive(false);
        
        newRecordObj.SetActive(false);
        newRecordBoxObj.SetActive(false);
    }

    public void ResultSkip()
    {
        if(canSkipResult)
        {
            foreach(Animation anim in animList)
            {
                AnimationState animState = anim[anim.clip.name];
                animState.time = anim.clip.length;

                anim.Sample();
            }

            return;
        }

        canSkipResult = true;
    }

    public bool IsShowed()
    {
        return rewardBoxObj.activeSelf;
    }
}
