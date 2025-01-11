using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] TMP_Text resultScoreText;
    [SerializeField] TMP_Text resultBestScoreText;
    [SerializeField] TMP_Text rewardVpText;
    [SerializeField] GameObject rewardBoxObj, rewardObj;

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

    public void OnResult()
    {
        resultAd.RequestAd();
    }

    public void ResetUI()
    {
        rewardBoxObj.SetActive(false);
        rewardObj.SetActive(false);
    }

    public bool IsShowed()
    {
        return rewardBoxObj.activeSelf;
    }
}
