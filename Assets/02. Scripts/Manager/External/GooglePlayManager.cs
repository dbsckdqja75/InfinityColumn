using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif

[RequireComponent(typeof(GoogleSavedGameUpdater))]
public class GooglePlayManager : MonoBehaviour
{
    GoogleSavedGameUpdater updater;

    #if UNITY_ANDROID
    [Serializable]
    class GoogleSavedData
    {
        public int voxelPoint, cashPoint;
        public int infinityBestScore, secondTimeBestScore, oneMinBestScore;

        public List<CharacterID> ownedCharacterData = new List<CharacterID>();
        public List<ConsumeID> purchasedConsumeList = new List<ConsumeID>();
    }

    bool isAuthorized = false;

    const string dataFileName = "Production_SavedGameData";

    Action onSignEvent = null;

    void Awake()
    {
        updater = this.GetComponent<GoogleSavedGameUpdater>();
    }

    void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        SignIn();
    }

    void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void ManuallySignIn()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    void OnSignIn()
    {
        isAuthorized = true;

        bool isLoadedCloudData = PlayerPrefsManager.LoadData("LoadedCloudData", false);
        if(!isLoadedCloudData)
        {
            LoadGameData();
        }

        onSignEvent?.Invoke();
        onSignEvent = null;
    }

    void SaveGameData()
    {
        if(isAuthorized)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

            savedGameClient.OpenWithAutomaticConflictResolution(dataFileName, DataSource.ReadCacheOrNetwork,
                                                                ConflictResolutionStrategy.UseLastKnownGood, OnSavedGameData);

        }
    }

    void LoadGameData()
    {
        if(isAuthorized)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

            savedGameClient.OpenWithAutomaticConflictResolution(dataFileName, DataSource.ReadCacheOrNetwork,
                                                                ConflictResolutionStrategy.UseLastKnownGood, OnLoadGameData);
        }
    }

    void OnSavedGameData(SavedGameRequestStatus status, ISavedGameMetadata gameData)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            var update = new SavedGameMetadataUpdate.Builder().Build();

            GoogleSavedData savedGameData = new GoogleSavedData();
            savedGameData.voxelPoint = PlayerPrefsManager.LoadData(CurrencyType.VOXEL_POINT.ToString(), 0);
            savedGameData.cashPoint = PlayerPrefsManager.LoadData(CurrencyType.CASH_POINT.ToString(), 0);
            
            savedGameData.infinityBestScore = PlayerPrefsManager.LoadData("BestScore", 0);
            savedGameData.secondTimeBestScore = PlayerPrefsManager.LoadData("SecondTimeBestScore", 0);
            savedGameData.oneMinBestScore = PlayerPrefsManager.LoadData("OneTimeBestScore", 0);

            string phrase = PlayerPrefsManager.LoadData("PlayerCharacterData", "0");
            string[] splitData = phrase.Split(',');

            foreach(string characterData in splitData)
            {
                int id;
                if(int.TryParse(characterData, out id))
                {
                    if(!savedGameData.ownedCharacterData.Contains((CharacterID)id))
                    {
                        savedGameData.ownedCharacterData.Add((CharacterID)id);
                    }
                }
            }

            // TODO : PurchasedConsumeData Load해서 넣고 저장하기
            savedGameData.purchasedConsumeList = new List<ConsumeID>();

            var json = JsonUtility.ToJson(savedGameData);
            byte[] data = Encoding.UTF8.GetBytes(json);

            savedGameClient.CommitUpdate(gameData, update, data, OnSavedGameWritten);

            return;
        }
    }

    void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata gameData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            return;
        }
    }

    void OnLoadGameData(SavedGameRequestStatus status, ISavedGameMetadata gameData)
    {
        ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            saveGameClient.ReadBinaryData(gameData, OnLoadedGameData);
            return;
        }
    }

    void OnLoadedGameData(SavedGameRequestStatus status, byte[] gameData)
    {
        string loadedData = Encoding.UTF8.GetString(gameData);
        if (loadedData != "")
        {
            GoogleSavedData savedGameData = JsonUtility.FromJson<GoogleSavedData>(loadedData);
            PlayerPrefsManager.SaveData(CurrencyType.VOXEL_POINT.ToString(), savedGameData.voxelPoint);
            PlayerPrefsManager.SaveData(CurrencyType.CASH_POINT.ToString(), savedGameData.cashPoint);

            PlayerPrefsManager.SaveData("BestScore", savedGameData.infinityBestScore);
            PlayerPrefsManager.SaveData("SecondTimeBestScore", savedGameData.secondTimeBestScore);
            PlayerPrefsManager.SaveData("OneTimeBestScore", savedGameData.oneMinBestScore);

            StringBuilder stringBuilder = new StringBuilder();
            foreach(CharacterID id in savedGameData.ownedCharacterData)
            {
                stringBuilder.Append(((int)id).ToString());
                if(savedGameData.ownedCharacterData.Count > 1)
                {
                    stringBuilder.Append(',');
                }
            }

            PlayerPrefsManager.SaveData("PlayerCharacterData", stringBuilder.ToString());

            stringBuilder = new StringBuilder();
            foreach(ConsumeID id in savedGameData.purchasedConsumeList)
            {
                stringBuilder.Append(((int)id).ToString());
                if(savedGameData.purchasedConsumeList.Count > 1)
                {
                    stringBuilder.Append(',');
                }
            }

            PlayerPrefsManager.SaveData("PurchasedConsumeData", stringBuilder.ToString());

            updater.OnLoadedGameData();

            PlayerPrefsManager.SaveData("LoadedCloudData", true);

            return;
        }

        if(loadedData == null || loadedData == "")
        {
            PlayerPrefsManager.SaveData("LoadedCloudData", true);

            SaveGameData();
        }
    }

    public void ShowAchievementUI()
	{
        if(isAuthorized)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            onSignEvent = () => PlayGamesPlatform.Instance.ShowAchievementsUI();
            ManuallySignIn();
        }
	}

    public void ShowLeaderboardUI()
	{
        if(isAuthorized)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            onSignEvent = () => PlayGamesPlatform.Instance.ShowLeaderboardUI();
            ManuallySignIn();
        }
	}

    void ReportLeaderboard(string boardID, int score)
    {
        if(isAuthorized)
        {
            PlayGamesPlatform.Instance.ReportScore(score, boardID, (bool success) => {});
        }
    }

    public void AllReportLeaderboard()
    {
        ReportLeaderboard(GameType.INFINITY, PlayerPrefsManager.LoadData("BestScore", 0));
        ReportLeaderboard(GameType.SECOND_ATTACK, PlayerPrefsManager.LoadData("SecondTimeBestScore", 0));
        ReportLeaderboard(GameType.ONE_MIN_ATTACK, PlayerPrefsManager.LoadData("OneTimeBestScore", 0));
    }

    public void ReportLeaderboard(GameType gameType, int score)
    {
        switch(gameType)
        {
            case GameType.INFINITY:
            ReportLeaderboard(GPGSIds.leaderboard_top_score_infinity, score);
            break;
            case GameType.SECOND_ATTACK:
            ReportLeaderboard(GPGSIds.leaderboard_top_score_30sec, score);
            break;
            case GameType.ONE_MIN_ATTACK:
            ReportLeaderboard(GPGSIds.leaderboard_top_score_1min, score);
            break;
            default:
            break;
        }
    }

    public void ReportGameData()
    {
        if(isAuthorized)
        {
            SaveGameData();
        }
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            OnSignIn();
        }
        else
        {
            onSignEvent = null;
        }
    }

    public bool IsAuthorized()
    {
        return isAuthorized;
    }
    #endif

    #if !UNITY_ANDROID
    void OnEnable() 
    {
        Destroy(this.gameObject);
    }
    #endif
}
