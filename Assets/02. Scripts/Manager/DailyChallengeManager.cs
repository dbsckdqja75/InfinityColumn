using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class DailyChallengeManager : MonoBehaviour
{
    [SerializeField] LocalizeText challengeText;
    [SerializeField] TMP_Text rewardText;
    [SerializeField] GameObject rewardObj;
    [SerializeField] GameObject clearObj;
    [SerializeField] GameObject challengeButtonObj;
    [SerializeField] Animation challengeAnim;

    bool isClear;
    ChallengeType currentChallenge = ChallengeType.NONE;

    SecureValue<int> goal = new SecureValue<int>(0);
    SecureValue<int> reward = new SecureValue<int>(0);

    void Awake() 
    {
        Init();
    }

    void Start()
    {
        this.StopAllCoroutines();

        DateCheck().Start(this);
    }

    void Init()
    {
        challengeButtonObj.SetActive(false);
    }

    void UpdateDate()
    {
        DateTime lastDateTime = PlayerPrefsManager.LoadData("LastChallengeDate", DateManager.Instance.todayDateTime.AddDays(-1));
        if(DateManager.Instance.IsDateEqual(lastDateTime) == false)
        {
            ResetChallenge();

            PlayerPrefsManager.SaveData("LastChallengeDate", DateManager.Instance.todayDateTime);
        }

        ActiveChallenge();
    }

    public void UpdateUI()
    {
        if(isClear)
        {
            clearObj.SetActive(true);

            rewardText.text = string.Format("+VP {0}", reward.GetValue());
            rewardObj.SetActive(true);
        }

        challengeAnim.playAutomatically = !isClear;

        challengeText.SetLocaleString(GetChallengeContext());
        challengeText.SetStringFormatValue(goal.GetValue().ToString());
    }

    void ActiveChallenge()
    {
        isClear = PlayerPrefsManager.LoadData("DailyChallengeClear", false);
        currentChallenge = (ChallengeType)((int)PlayerPrefsManager.LoadData("DailyChallengeType", 0));
        goal.SetValue(PlayerPrefsManager.LoadData("DailyChallengeGoal", 0));
        reward.SetValue(PlayerPrefsManager.LoadData("DailyChallengeReward", 0));

        challengeAnim.playAutomatically = !isClear;

        challengeButtonObj.SetActive(true);

        Debug.LogFormat("[DailyChallenge] Type : {0} / Goal : {1} / Reward : {2} / isClear : {3}", currentChallenge.ToString(), goal.GetValue(), reward.GetValue(), isClear);
    }

    public void UpdateChallenge(GameType gameType, int record)
    {
        if(!isClear)
        {
            switch(currentChallenge)
            {
                case ChallengeType.NEW_RECORD_INFINITY_100:
                    if(gameType.IsEquals(GameType.INFINITY) && record >= goal.GetValue())
                    {
                        ClearChallenge();
                    }
                    break;
                case ChallengeType.NEW_RECORD_ONE_MIN_100:
                    if(gameType.IsEquals(GameType.ONE_TIME_ATTACK) && record >= goal.GetValue())
                    {
                        ClearChallenge();
                    }
                    break;
                case ChallengeType.NEW_RECORD_THREE_MIN_100:
                    if(gameType.IsEquals(GameType.THREE_TIME_ATTACK) && record >= goal.GetValue())
                    {
                        ClearChallenge();
                    }
                    break;
                default:
                    DateCheck().Start(this);
                    break;
            }

            Debug.LogFormat("[DailyChallenge] UpdateChallenge {0} : {1}", gameType, record);
        }
    }

    void ClearChallenge()
    {
        if(!isClear)
        {
            isClear = true;

            CurrencyManager.Instance.RewardCurrency(CurrencyType.VOXEL_POINT, reward.GetValue());

            PlayerPrefsManager.SaveData("DailyChallengeClear", true);

            Debug.LogFormat("[DailyChallenge] Challenge Clear Reward : {0}", reward.GetValue());
        }
    }

    void ResetChallenge()
    {
        ChallengeType newChallenge = (ChallengeType)Random.Range(1, 3+1);
        switch(newChallenge)
        {
            case ChallengeType.NEW_RECORD_INFINITY_100:
                goal.SetValue(PlayerPrefsManager.LoadData("BestScore", 0) + 100);
                reward.SetValue(100);
                break;
            case ChallengeType.NEW_RECORD_ONE_MIN_100:
                goal.SetValue(PlayerPrefsManager.LoadData("OneTimeBestScore", 0) + 100);
                reward.SetValue(100);
                break;
            case ChallengeType.NEW_RECORD_THREE_MIN_100:
                goal.SetValue(PlayerPrefsManager.LoadData("ThreeTimeBestScore", 0) + 100);
                reward.SetValue(100);
                break;
            default:
                return;
        }

        PlayerPrefsManager.SaveData("DailyChallengeClear", false);
        PlayerPrefsManager.SaveData("DailyChallengeType", ((int)newChallenge));
        PlayerPrefsManager.SaveData("DailyChallengeGoal", goal.GetValue());
        PlayerPrefsManager.SaveData("DailyChallengeReward", reward.GetValue());
    }

    string GetChallengeContext()
    {
        switch(currentChallenge)
        {
            case ChallengeType.NEW_RECORD_INFINITY_100:
                return "DailyChallenge_Infinity_100";
            case ChallengeType.NEW_RECORD_ONE_MIN_100:
                return "DailyChallenge_OneMin_100";
            case ChallengeType.NEW_RECORD_THREE_MIN_100:
                return "DailyChallenge_ThreeMin_100";
            default:
                return "None";
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
            int bestScore = PlayerPrefsManager.LoadData("BestScore", 0);
            if(bestScore >= 100)
            {
                UpdateDate();
            }
        }

        yield break;
    }
}
