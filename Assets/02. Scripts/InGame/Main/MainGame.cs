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

        disturbManager.ResetTrigger();
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
        bool isNewRecord = false;
        if(bestScore.GetValue() < score.GetValue())
        {
            if(score.GetValue() < 999999) // NOTE : 최대 점수 임시 제한 (조작 방지)
            {
                isNewRecord = true;

                bestScore.SetValue(score.GetValue());

                SaveGameRecord();
            }
        }

        resultUI.ReportScore(score.GetValue(), bestScore.GetValue());
        resultUI.OnResult(isNewRecord);
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

        gameManager.OnSaveRecord();
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

    public string GetGameModeStringKey()
    {
        return gameModeStringKey;
    }

    public bool IsCurrentState(GameState targetState)
    {
        return currentState.IsEquals(targetState);
    }
}
