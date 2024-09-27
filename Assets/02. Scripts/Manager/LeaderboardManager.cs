using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [Serializable]
    class BestScoreData
    {
        public BestScoreData(string name, int score)
        {
            this.playerName = name;
            this.bestScore = score;
        }

        public string playerName;
        public int bestScore;
    }

    [Serializable]
    class LeaderboardScoreData
    {
        public List<BestScoreData> data;
    }

    [SerializeField] LocalizeText currentGameTypeText;

    [SerializeField] TMP_Text[] rankTexts;
    [SerializeField] GameObject rankListObj;

    [SerializeField] TMP_Text localPlayerRankText;

    [SerializeField] TMP_InputField nickNameInputField;
    [SerializeField] Button nickNameConfirmButton;
    
    [SerializeField] GameObject emptyDataObj;
    [SerializeField] GameObject failLoadObj;
    [SerializeField] GameObject loadingObj;
    [SerializeField] GameObject firstGuideObj;

    GameType currentGameType;

    LeaderboardScoreData leaderboardData;

    string localPlayerName;

    void Awake() 
    {
        localPlayerName = PlayerPrefsManager.LoadData("LocalPlayerName", "");
    }

    public void OnLeaderboard()
    {
        currentGameType = GameType.INFINITY;

        UpdateGameTypeTitle();

        this.StopAllCoroutines();

        UpdateLeaderboard();
    }

    void UpdateLeaderboard()
    {
        HideLocalPlayerRank();
        HideLeaderboard();

        leaderboardData = new LeaderboardScoreData();
        leaderboardData.data = new List<BestScoreData>();
        leaderboardData.data.Clear();

        switch(currentGameType)
        {
            case GameType.ONE_TIME_ATTACK:
            GetLeaderboardRequest("http://bu1ld.asuscomm.com:8002/OneMinBestScoreTop10").Start(this);
            break;
            case GameType.THREE_TIME_ATTACK:
            GetLeaderboardRequest("http://bu1ld.asuscomm.com:8002/ThreeMinBestScoreTop10").Start(this);
            break;
            default:
            GetLeaderboardRequest("http://bu1ld.asuscomm.com:8002/InfinityBestScoreTop10").Start(this);
            break;
        }

        if(localPlayerName.Length > 0)
        {
            AllUpdateRecord().Start(this);
            UpdateLocalPlayerRank();
        }
        else
        {
            ShowFirstGuide();
        }
    }

    void OnEmptyLeaderboard()
    {
        HideLocalPlayerRank();

        rankListObj.SetActive(false);
        emptyDataObj.SetActive(true);
        loadingObj.SetActive(false);
    }

    void OnFailLoadLeaderboard()
    {
        HideLocalPlayerRank();

        rankListObj.SetActive(false);
        emptyDataObj.SetActive(false);
        failLoadObj.SetActive(true);
        loadingObj.SetActive(false);
    }

    void ShowLeaderboard()
    {
        if(leaderboardData.data.Count > 0)
        {
            foreach(TMP_Text tmpText in rankTexts)
            {
                tmpText.text = "";
            }

            for(int i = 0; i < leaderboardData.data.Count; i++)
            {
                BestScoreData data = leaderboardData.data[i];
                if(data.playerName.Length > 0)
                {
                    rankTexts[i].text = string.Format("{0}. {1} ({2:N0})", (i+1), data.playerName, data.bestScore);
                }
            }

            rankListObj.SetActive(true);
            loadingObj.SetActive(false);
        }
        else
        {
            OnEmptyLeaderboard();
        }
    }

    void HideLeaderboard()
    {
        rankListObj.SetActive(false);
        emptyDataObj.SetActive(false);
        failLoadObj.SetActive(false);
        loadingObj.SetActive(true);
    }

    void ShowLocalPlayerRank()
    {
        localPlayerRankText.gameObject.SetActive(localPlayerRankText.text.Length > 0);
    }

    void HideLocalPlayerRank()
    {
        localPlayerRankText.gameObject.SetActive(false);
    }

    public void PreviousTargetGameType()
    {
        currentGameType = currentGameType.Previous();

        this.StopAllCoroutines();

        UpdateGameTypeTitle();
        UpdateLeaderboard();
    }

    public void NextTargetGameType()
    {
        currentGameType = currentGameType.Next();

        this.StopAllCoroutines();

        UpdateGameTypeTitle();
        UpdateLeaderboard();
    }

    void UpdateGameTypeTitle()
    {
        switch(currentGameType)
        {
            case GameType.ONE_TIME_ATTACK:
            currentGameTypeText.SetLocaleString("Leaderboard_OneTimeAttack");
            break;
            case GameType.THREE_TIME_ATTACK:
            currentGameTypeText.SetLocaleString("Leaderboard_ThreeTimeAttack");
            break;
            default:
            currentGameTypeText.SetLocaleString("Leaderboard_Infinity");
            break;
        }
    }

    void ShowFirstGuide()
    {
        nickNameInputField.text = "";
        UpdateNicknameValidation();

        firstGuideObj.SetActive(true);
    }

    public void UpdateNicknameValidation()
    {
        nickNameConfirmButton.interactable = (nickNameInputField.text.Trim().Length > 0);
    }

    public void ConfirmNickname()
    {
        localPlayerName = nickNameInputField.text.Trim();
        PlayerPrefsManager.SaveData("LocalPlayerName", localPlayerName);

        firstGuideObj.SetActive(false);

        UpdateLeaderboard();
    }

    void UpdateLocalPlayerRank()
    {
        localPlayerRankText.text = "";

        switch(currentGameType)
        {
            case GameType.ONE_TIME_ATTACK:
            GetPlayerRankRequest("http://bu1ld.asuscomm.com:8002/OneMinBestScoreRankWithScore").Start(this);
            break;
            case GameType.THREE_TIME_ATTACK:
            GetPlayerRankRequest("http://bu1ld.asuscomm.com:8002/ThreeMinBestScoreRankWithScore").Start(this);
            break;
            default:
            GetPlayerRankRequest("http://bu1ld.asuscomm.com:8002/InfinityBestScoreRankWithScore").Start(this);
            break;
        }
    }

    public void UpdateRecord(GameType targetGameType)
    {
        if(localPlayerName.Length > 0)
        {
            switch(targetGameType)
            {
                case GameType.ONE_TIME_ATTACK:
                UpdateRecord("OneTimeBestScore", "http://bu1ld.asuscomm.com:8002/UpdateOneMinBestScore").Start(this);
                Debug.Log("T");
                break;
                case GameType.THREE_TIME_ATTACK:
                UpdateRecord("ThreeTimeBestScore", "http://bu1ld.asuscomm.com:8002/UpdateThreeMinBestScore").Start(this);
                break;
                default:
                UpdateRecord("BestScore", "http://bu1ld.asuscomm.com:8002/UpdateInfinityBestScore").Start(this);
                break;
            }
        }
    }

    IEnumerator AllUpdateRecord()
    {
        yield return StartCoroutine(UpdateRecord("BestScore", "http://bu1ld.asuscomm.com:8002/UpdateInfinityBestScore"));
        yield return StartCoroutine(UpdateRecord("OneTimeBestScore", "http://bu1ld.asuscomm.com:8002/UpdateOneMinBestScore"));
        yield return StartCoroutine(UpdateRecord("ThreeTimeBestScore", "http://bu1ld.asuscomm.com:8002/UpdateThreeMinBestScore"));
        yield break;
    }

    IEnumerator UpdateRecord(string dataKey, string url)
    {
        int bestScore = PlayerPrefsManager.LoadData(dataKey, 0);
        BestScoreData data = new BestScoreData(localPlayerName, bestScore);
        yield return StartCoroutine(UpdateLeaderboardRequest(url, data));
        yield break;
    }

    IEnumerator GetLeaderboardRequest(string url) // NOTE : 랭크 TOP 10 리스트로 가져오기
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 10;

            yield return request.SendWebRequest();
            yield return new WaitUntil(() => request.isDone);

            if(request.result.IsEquals(UnityWebRequest.Result.Success))
            {
                leaderboardData = JsonUtility.FromJson<LeaderboardScoreData>(request.downloadHandler.text);

                ShowLeaderboard();
            }
            else
            {
                OnFailLoadLeaderboard();

                Debug.LogWarningFormat("[LeaderboardManager] DB 서버 접근 실패 : {0}", request.error);
            }

            request.Dispose();
        }

        yield break;
    }

    IEnumerator GetPlayerRankRequest(string url) // NOTE : 플레이어 순위와 기록 가져오기
    {
        WWWForm form = new WWWForm();
        form.AddField("playerName", localPlayerName);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.timeout = 10;

            yield return request.SendWebRequest();
            yield return new WaitUntil(() => request.isDone);

            if(request.result.IsEquals(UnityWebRequest.Result.Success))
            {
                if(request.downloadHandler.text != "0")
                {
                    string[] data = request.downloadHandler.text.Replace("\"", "").Split(',');
                    int rank = int.Parse(data[0]);
                    int record = int.Parse(data[1]);

                    localPlayerRankText.text = string.Format("{0}. {1} ({2:N0})", rank, localPlayerName, record);

                    ShowLocalPlayerRank();
                }
            }
            else
            {
                OnFailLoadLeaderboard();

                Debug.LogWarningFormat("[LeaderboardManager] DB 서버 접근 실패 : {0}", request.error);
            }

            request.Dispose();
        }

        yield break;
    }

    IEnumerator UpdateLeaderboardRequest(string url, BestScoreData data) // NOTE : 플레이어 최고 점수 보내기
    {
        #if !UNITY_EDITOR
        WWWForm form = new WWWForm();
        form.AddField("playerName", data.playerName);
        form.AddField("bestScore", data.bestScore);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.timeout = 10;

            yield return request.SendWebRequest();
            yield return new WaitUntil(() => request.isDone);

            if(request.result.IsEquals(UnityWebRequest.Result.Success))
            {
                Debug.LogFormat("[LeaderboardManager] 리더보드 업데이트 완료 : {0} - {1}", data.playerName, data.bestScore);
            }
            else
            {
                Debug.LogWarningFormat("[LeaderboardManager] DB 서버 접근 실패 : {0}", request.error);
            }

            request.Dispose();
        }
        #else
            Debug.Log("[LeaderboardManager] 에디터 리더보드 업데이트 방지");
        #endif

        yield break;
    }
}
