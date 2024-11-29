using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using TMPro;


#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif

public class GooglePlayManager : MonoBehaviour
{
    [SerializeField] GameObject testObj;

    [SerializeField] TMP_Text statsText;

    [SerializeField] GoogleSavedGameUpdater updater;

    #if UNITY_ANDROID
    [Serializable]
    class GoogleSavedData
    {
        public int voxelPoint, cashPoint;
        public int infinityBestScore, oneMinBestScore, threeMinBestScore;
        public string currentCharacterID;
        public List<CharacterID> ownedCharacterData = new List<CharacterID>();
        public List<ConsumeID> purchasedConsumeList = new List<ConsumeID>();
    }

    const string dataFileName = "ProjectIC_SavedGameData";

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

    void OnSignIn()
    {
        statsText.text = "로그인 성공";

        bool isLoadedCloudData = PlayerPrefsManager.LoadData("LoadedCloudData", false);
        if(!isLoadedCloudData)
        {
            statsText.text = "로그인 성공 + 데이터 초기 로드";

            LoadGameData();
        }
    }

    void SaveGameData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(dataFileName, DataSource.ReadCacheOrNetwork,
                                                            ConflictResolutionStrategy.UseLastKnownGood, OnSavedGameData);
    }

    void LoadGameData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(dataFileName, DataSource.ReadCacheOrNetwork,
                                                            ConflictResolutionStrategy.UseLastKnownGood, OnLoadGameData);
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
            savedGameData.oneMinBestScore = PlayerPrefsManager.LoadData("OneTimeBestScore", 0);
            savedGameData.threeMinBestScore = PlayerPrefsManager.LoadData("ThreeTimeBestScore", 0);

            savedGameData.currentCharacterID = PlayerPrefsManager.LoadData("CurrentCharacterData", "0");

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

        Debug.Log("[GPGS] 게임 데이터 저장 실패");
    }

    void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata gameData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            statsText.text = "저장 성공";

            Debug.Log("[GPGS] 게임 데이터 저장 성공");

            return;
        }

        Debug.Log("[GPGS] 게임 데이터 저장 쓰기 실패");
    }

    void OnLoadGameData(SavedGameRequestStatus status, ISavedGameMetadata gameData)
    {
        ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            saveGameClient.ReadBinaryData(gameData, OnLoadedGameData);

            return;
        }

        Debug.Log("[GPGS] 게임 데이터 불러오기 실패");
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
            PlayerPrefsManager.SaveData("OneTimeBestScore", savedGameData.oneMinBestScore);
            PlayerPrefsManager.SaveData("ThreeTimeBestScore", savedGameData.threeMinBestScore);

            PlayerPrefsManager.SaveData("CurrentCharacterData", savedGameData.currentCharacterID);

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

            statsText.text = "로드 성공";

            Debug.Log("[GPGS] 게임 데이터 불러오기 성공");

            return;
        }

        if(loadedData == null || loadedData == "")
        {
            PlayerPrefsManager.SaveData("LoadedCloudData", true);

            SaveGameData();
        }

        Debug.Log("[GPGS] 게임 데이터 불러오기 실패 (변환 불가)");
    }

    public void ShowAchievementUI()
	{
		PlayGamesPlatform.Instance.ShowAchievementsUI();
	}

    public void ShowLeaderboardUI()
	{
		PlayGamesPlatform.Instance.ShowLeaderboardUI();
	}

    public void DebugLoad()
    {
        LoadGameData();
    }

    public void DebugSave()
    {
        SaveGameData();
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            OnSignIn();

            testObj.SetActive(true);

            Debug.Log("[GPGS] 로그인 성공");
        }
        else
        {
            testObj.SetActive(false);

            Debug.Log("[GPGS] 로그인 실패");
        }
    }
    #else
    void Awake() 
    {
        Destroy(this.gameObject);
    }
    #endif
}
