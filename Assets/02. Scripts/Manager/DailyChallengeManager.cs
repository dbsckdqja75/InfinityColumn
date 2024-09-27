using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

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

    int _goal, _reward;
    int goal { get { return AntiCheatManager.SecureInt(_goal); } set { _goal = AntiCheatManager.SecureInt(value); } }
    int reward { get { return AntiCheatManager.SecureInt(_reward); } set { _reward = AntiCheatManager.SecureInt(value); } }

    void Awake() 
    {
        Init();
    }

    void Start()
    {
        WebTimeCheck().Start(this);
    }

    void Init()
    {
        challengeButtonObj.SetActive(false);
    }

    void UpdateDateTime(DateTime todayDateTime)
    {
        DateTime lastDateTime = PlayerPrefsManager.LoadData("LastChallengeDate", todayDateTime.AddDays(-1));
        string lastDay = string.Format("{0:yyyy-MM-dd}", lastDateTime);
        string today = string.Format("{0:yyyy-MM-dd}", todayDateTime);

        if(today != lastDay)
        {
            ResetChallenge();

            PlayerPrefsManager.SaveData("LastChallengeDate", todayDateTime);
        }

        ActiveChallenge();
    }

    public void UpdateUI()
    {
        if(isClear)
        {
            clearObj.SetActive(true);

            rewardText.text = string.Format("+VP {0}", reward);
            rewardObj.SetActive(true);
        }

        challengeAnim.playAutomatically = !isClear;

        challengeText.SetLocaleString(GetChallengeContext());
        challengeText.SetStringFormatValue(goal.ToString());
    }

    void ActiveChallenge()
    {
        isClear = PlayerPrefsManager.LoadData("DailyChallengeClear", false);
        currentChallenge = (ChallengeType)((int)PlayerPrefsManager.LoadData("DailyChallengeType", 0));
        goal = PlayerPrefsManager.LoadData("DailyChallengeGoal", 0);
        reward = PlayerPrefsManager.LoadData("DailyChallengeReward", 0);

        challengeAnim.playAutomatically = !isClear;

        challengeButtonObj.SetActive(true);

        Debug.LogFormat("[DailyChallenge] Type : {0} / Goal : {1} / Reward : {2} / isClear : {3}", currentChallenge.ToString(), goal, reward, isClear);
    }

    public void UpdateChallenge(GameType gameType, int record)
    {
        if(!isClear)
        {
            switch(currentChallenge)
            {
                case ChallengeType.NEW_RECORD_INFINITY_100:
                    if(gameType.IsEquals(GameType.INFINITY) && record >= goal)
                    {
                        ClearChallenge();
                    }
                    break;
                case ChallengeType.NEW_RECORD_ONE_MIN_100:
                    if(gameType.IsEquals(GameType.ONE_TIME_ATTACK) && record >= goal)
                    {
                        ClearChallenge();
                    }
                    break;
                case ChallengeType.NEW_RECORD_THREE_MIN_100:
                    if(gameType.IsEquals(GameType.THREE_TIME_ATTACK) && record >= goal)
                    {
                        ClearChallenge();
                    }
                    break;
                default:
                    WebTimeCheck().Start(this);
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

            CurrencyManager.Instance.RewardCurrency(CurrencyType.VOXEL_POINT, reward);

            PlayerPrefsManager.SaveData("DailyChallengeClear", true);

            Debug.LogFormat("[DailyChallenge] Challenge Clear Reward : {0}", reward);
        }
    }

    void ResetChallenge()
    {
        ChallengeType newChallenge = (ChallengeType)Random.Range(1, 3+1);
        switch(newChallenge)
        {
            case ChallengeType.NEW_RECORD_INFINITY_100:
                goal = PlayerPrefsManager.LoadData("BestScore", 0) + 100;
                reward = 100;
                break;
            case ChallengeType.NEW_RECORD_ONE_MIN_100:
                goal = PlayerPrefsManager.LoadData("OneTimeBestScore", 0) + 100;
                reward = 100;
                break;
            case ChallengeType.NEW_RECORD_THREE_MIN_100:
                goal = PlayerPrefsManager.LoadData("ThreeTimeBestScore", 0) + 100;
                reward = 100;
                break;
            default:
                return;
        }

        PlayerPrefsManager.SaveData("DailyChallengeClear", false);
        PlayerPrefsManager.SaveData("DailyChallengeType", ((int)newChallenge));
        PlayerPrefsManager.SaveData("DailyChallengeGoal", goal);
        PlayerPrefsManager.SaveData("DailyChallengeReward", reward);
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

    IEnumerator WebTimeCheck()
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get("www.google.com"))
        {
            yield return request.SendWebRequest();

            if (request.result.IsEquals(UnityWebRequest.Result.Success))
            {
                string date = request.GetResponseHeader("date");
                DateTime todayDateTime = DateTime.Parse(date).ToLocalTime();

                int bestScore = PlayerPrefsManager.LoadData("BestScore", 0);
                if(bestScore >= 100)
                {
                    UpdateDateTime(todayDateTime);
                }
            }
        }

        yield break;
    }
}
