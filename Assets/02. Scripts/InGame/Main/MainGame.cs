using UnityEngine;

public class MainGame : MonoBehaviour
{
    [SerializeField]
    GameType gameType = GameType.INFINITY;
    GameState currentState = GameState.LOBBY;

    protected GameManager gameManager;

    protected float health = 0;

    protected SecureValue<int> score = new SecureValue<int>(0);
    protected SecureValue<int> bestScore = new SecureValue<int>(0);

    [SerializeField]
    protected string bestScoreDataKey = "BestScore";
    
    [SerializeField]
    protected string gameModeStringKey = "GameMode_Infinity";

    protected PlayUI playUI;
    protected ResultUI resultUI;

    protected CameraView cameraView;
    protected PlayerController playerController;
    protected SpawnManager spawnManager;
    protected SkyBoxManager skyBoxManager;
    protected DisturbManager disturbManager;

    #if UNITY_EDITOR || UNITY_STANDALONE
    protected LeaderboardManager leaderboardManager;
    #endif

    #if UNITY_EDITOR || UNITY_ANDROID
    protected GooglePlayManager GPGS;
    #endif

    void Awake()
    {
        currentState = GameState.LOBBY;
    }

    public virtual void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        playUI = gameManager.GetPlayUI();
        resultUI = gameManager.GetResultUI();
        cameraView = gameManager.GetCameraView();
        playerController = gameManager.GetPlayerController();
        spawnManager = gameManager.GetSpawnManager();
        skyBoxManager = gameManager.GetSkyBoxManager();
        disturbManager = gameManager.GetDisturbManager();

        #if UNITY_EDITOR || UNITY_STANDALONE
        leaderboardManager = gameManager.GetLeaderboardManager();
        #endif

        #if UNITY_EDITOR || UNITY_ANDROID
        GPGS = gameManager.GetGooglePlayManager();
        #endif
    }

    public virtual void UpdateLogic()
    {
        if(IsCurrentState(GameState.PLAYING))
        {
            UpdateHealth();
        }
    }

    protected virtual void UpdateHealth()
    {
        if(health <= 0)
        {
            gameManager.GameOver();
            return;
        }
    }



    public virtual void OnGameReady()
    {
        currentState = GameState.READY;

        playerController.SetControlLock(false);

        cameraView.SetCameraPreset("Playing");
    }

    public void OnGameStart()
    {
        currentState = GameState.PLAYING;
    }

    public virtual void OnGameOver()
    {
        currentState = GameState.GAME_OVER;

        OnResultScore();

        disturbManager.ResetTrigger();

        cameraView.SetCameraPreset("GameOver");
    }

    public virtual void OnGameReset()
    {
        currentState = GameState.LOBBY;

        score.SetValue(0);

        LoadGameRecord();
    }



    public virtual void OnGameResume()
    {
        if(IsCurrentState(GameState.PAUSE))
        {
            currentState = GameState.PLAYING;
        }

        playerController.SetControlLock(false);
    }

    public void OnGamePause()
    {
        if(IsCurrentState(GameState.PLAYING))
        {
            currentState = GameState.PAUSE;
        }

        playerController.SetControlLock(true);
    }

    public bool OnExtraMenuOpen()
    {
        if(IsCurrentState(GameState.LOBBY))
        {
            currentState = GameState.EXTRA_MENU;

            playerController.SetControlLock(true);

            return true;
        }

        return false;
    }

    public void OnExtraMenuClosed()
    {
        currentState = GameState.LOBBY;
    }

    protected virtual void CheckBranch()
    {
        Column column = spawnManager.GetNextColumn();
        if(column.HasBranch(playerController.GetDirectionType()))
        {
            spawnManager.SpawnEffect(column);

            gameManager.GameOver();
        }
    }

    protected void RewardVP(int resultScore, int rewardStep)
    {
        if(resultScore >= rewardStep)
        {
            int rewardAmount = (resultScore / rewardStep);

            resultUI.ReportReward(rewardAmount);

            CurrencyManager.Instance.RewardCurrency(CurrencyType.VOXEL_POINT, rewardAmount);
        }
    }

    protected void EarnScore(int earnAmount = 1)
    {
        score.SetValue(score.GetValue() + earnAmount);

        playUI.UpdateScore(score.GetValue());
    }

    protected void OnResultScore()
    {
        if(bestScore.GetValue() < score.GetValue())
        {
            // TODO : 갱신 표시 효과 추가 (NEW)
            bestScore.SetValue(score.GetValue());

            SaveGameRecord();
        }

        resultUI.ReportScore(score.GetValue(), bestScore.GetValue());
        resultUI.OnResult();
    }

    public virtual void OnPlayerMoved()
    {
        if(IsCurrentState(GameState.READY))
        {
            OnGameStart();

            playUI.HideStartGuide();
        }

        if(IsCurrentState(GameState.PLAYING))
        {
            CheckBranch();

            spawnManager.NextColumn();
        }
    }

    public void LoadGameRecord()
    {
        bestScore.SetValue(PlayerPrefsManager.LoadData(bestScoreDataKey, 0));
    }

    protected void SaveGameRecord()
    {
        PlayerPrefsManager.SaveData(bestScoreDataKey, bestScore.GetValue());

        #if UNITY_ANDROID
            GPGS.ReportLeaderboard(gameType, bestScore.GetValue());
            GPGS.ReportGameData();
        #else
            leaderboardManager.UpdateRecord(gameType);
        #endif
    }

    public GameType GetGameType()
    {
        return gameType;
    }

    public int GetScore()
    {
        return score.GetValue();
    }

    public int GetBestScore()
    {
        return bestScore.GetValue();
    }

    public virtual string GetGameModeStringKey()
    {
        return gameModeStringKey;
    }

    public bool IsCurrentState(GameState targetState)
    {
        return currentState.IsEquals(targetState);
    }
}
