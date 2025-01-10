using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static readonly int PLAYABLE_HEIGHT_LIMIT = 1000;

    GameState gameState = GameState.LOADING;
    GameType gameType = GameType.INFINITY;
    GameLogic currentGame = new InfinityLogic();

    float health = 100f;

    SecureValue<int> score = new SecureValue<int>(0);
    SecureValue<int> bestScore = new SecureValue<int>(0);

    bool isFeverTime = false;
    float feverTimer = 0f;
    SecureValue<int> feverCharge = new SecureValue<int>(0);

    float lastPlayerHeight;

    [Header("InGame Config")]
    const float regainHealth = 4;
    const int reChargeFever = 1;
    const float regainFeverTime = 1;
    const int rewardStep = 100;

    [SerializeField] int maxFever = 100;
    [SerializeField] float feverDuration = 5f;
    float feverTimeRate = 0f;

    [Header("InGame UI")]
    [SerializeField] PlayUI playUI;
    [SerializeField] ResultUI resultUI;

    [SerializeField] LocalizeText gameModeText;

    [Space(10)]
    [SerializeField] CameraView cameraView;
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] BackgroundManager backgroundManager;
    [SerializeField] SkyBoxManager skyBoxManager;
    [SerializeField] DisturbManager disturbManager;
    [SerializeField] CanvasManager canvasManager;
    [SerializeField] DailyChallengeManager dailyChallengeManager;
    [SerializeField] LeaderboardManager leaderboardManager;
    [SerializeField] GooglePlayManager GPGS;

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if(IsGameState(GameState.PLAYING))
        {
            UpdateHealthTime();
            playUI.UpdateHealth(health / currentGame.GetMaxHealth());

            UpdateFeverTime();
            playUI.UpdateFever(feverTimer / maxFever);
        }

        if(IsGameState(GameState.GAME_OVER))
        {
            UpdatePlayerHeight();
        }
    }

    void Init()
    {
        feverTimeRate = maxFever * (1 / feverDuration);

        cameraView.Init();
        spawnManager.Init();
        playerController.Init();

        ReturnLobby();

        playerController.SetControlLock(true);
        playerController.AddMoveEvent(() => { OnPlayerMove(); });

        LoadGameData();
    }

    void UpdateHealthTime()
    {
        if(health <= 0)
        {
            GameOver();
            return;
        }

        if(score.GetValue() > 0)
        {
            health -= (currentGame.GetTimePerDamage() * Time.deltaTime);
            health = Mathf.Clamp(health, 0f, (float)currentGame.GetMaxHealth());
        }

        playUI.UpdateHealthTimer((int)health);
    }

    void UpdateFeverTime()
    {
        if(isFeverTime)
        {
            health = currentGame.GetMaxHealth();

            if(feverTimer > 0f)
            {
                feverTimer = Mathf.Clamp(feverTimer, 0f, maxFever);
                feverTimer -= (feverTimeRate * Time.deltaTime);
            }
            else
            {
                ResetFeverTime();
            }
        }
    }

    void UpdateBestScore()
    {
        bestScore.SetValue(PlayerPrefsManager.LoadData(currentGame.GetBestScoreDataKey(), 0));
    }

    void UpdateResultScore()
    {
        if(bestScore.GetValue() < score.GetValue())
        {
            // TODO : 갱신 표시 효과 추가 (신기록 달성)
            bestScore.SetValue(score.GetValue());

            SaveGameData();
        }

        resultUI.ReportScore(score.GetValue(), bestScore.GetValue());
    }

    void UpdatePlayerHeight()
    {
        float t = (playerController.GetPlayerHeight() / lastPlayerHeight);
        if(t <= 0.1f)
        {
            skyBoxManager.FixHeight(0);
            
            return;
        }

        int fallScore = (int)Mathf.Lerp(0, score.GetValue(), t);

        skyBoxManager.UpdateHeight(fallScore);
    }

    void ResetUI()
    {
        playUI.ResetUI();
        resultUI.ResetUI();

        CurrencyLabel.refresh?.Invoke();
    }

    public void UpdateBestScoreData()
    {
        UpdateBestScore();
    }

    public void ChangeGameMode()
    {
        gameType = gameType.Next();
        switch(gameType)
        {
            case GameType.ONE_TIME_ATTACK:
            currentGame = new OneTimeLogic();
            break;
            case GameType.THREE_TIME_ATTACK:
            currentGame = new ThreeTimeLogic();
            break;
            default:
            currentGame = new InfinityLogic();
            break;
        }

        currentGame.OnChangeGameMode(playUI);

        gameModeText.SetLocaleString(currentGame.GetGameModeStringKey());

        UpdateBestScore();

        ReturnLobby();
    }

    public void GameStart()
    {
        if(IsGameState(GameState.LOBBY))
        {
            gameState = GameState.PLAYING;

            playerInput.SwitchCurrentActionMap("PlayerActions");

            cameraView.SetCameraPreset("Playing");

            playerController.SetControlLock(false);

            canvasManager.SetPanel("Playing");
        }
    }

    void GameOver()
    {
        gameState = GameState.GAME_OVER;

        cameraView.SetCameraPreset("GameOver");

        lastPlayerHeight = playerController.GetPlayerHeight();
        playerController.SetControlLock(true);
        playerController.OnFall();

        resultUI.OnResult();

        OnReward(score.GetValue());
        UpdateResultScore();
    
        dailyChallengeManager.UpdateChallenge(gameType, bestScore.GetValue());

        playerInput.SwitchCurrentActionMap("ResultActions");

        disturbManager.ResetTrigger();

        canvasManager.SetPanel("GameOver");

        SoundManager.Instance.StopMusic();
    }

    void OnReward(int score)
    {
        if(score >= rewardStep)
        {
            int reward = (score / rewardStep);

            resultUI.ReportReward(reward);

            CurrencyManager.Instance.RewardCurrency(CurrencyType.VOXEL_POINT, reward);
        }
    }

    void LoadGameData()
    {
        bestScore.SetValue(PlayerPrefsManager.LoadData(currentGame.GetBestScoreDataKey(), 0));
    }

    void SaveGameData()
    {
        PlayerPrefsManager.SaveData(currentGame.GetBestScoreDataKey(), bestScore.GetValue());
        
        #if UNITY_ANDROID
            GPGS.ReportLeaderboard(gameType, bestScore.GetValue());
            GPGS.ReportGameData();
        #else
            leaderboardManager.UpdateRecord(gameType);
        #endif
    }

    public void GameRestart()
    {
        ReturnLobby();
        GameStart();
    }

    public void GamePause()
    {
        if(IsGameState(GameState.PLAYING))
        {
            gameState = GameState.PAUSE;

            playerController.SetControlLock(true);
        
            canvasManager.SetPanel("Pause");

            SoundManager.Instance.PlayMusic("Infinity Pause");
        }
    }

    public void GameResume()
    {
        if(IsGameState(GameState.PAUSE))
        {
            gameState = GameState.PLAYING;

            playerController.SetControlLock(false);

            canvasManager.SetPanel("Playing");

            SoundManager.Instance.PlayMusic("Infinity Music");
        }
    }

    public void ReturnLobby()
    {
        GameReset();

        cameraView.SetCameraPreset("Lobby");
        if(!IsGameState(GameState.PLAYING))
        {
            cameraView.RepositionView();
        }

        if(!IsGameState(GameState.LOBBY))
        {
            SoundManager.Instance.PlayMusic("Infinity Music");
        }

        gameState = GameState.LOBBY;

        playerInput.SwitchCurrentActionMap("LobbyActions");

        ResetUI();

        canvasManager.SetPanel("Lobby");
    }

    void GameReset()
    {
        score.SetValue(0);

        currentGame.OnGameReset();

        health = currentGame.GetMaxHealth();

        ResetFeverTime();

        playUI.ResetUI();

        playerController.ResetPosition();

        backgroundManager.ResetBackground();

        spawnManager.ResetPool();
        spawnManager.UpdateColumnMode(false);

        skyBoxManager.ResetColor();
        skyBoxManager.ResetHeight();

        disturbManager.ResetTrigger();
    }

    void ResetFeverTime()
    {
        isFeverTime = false;

        feverTimer = 0;
        feverCharge.SetValue(0);

        playerController.OnFever(false);

        playUI.OnResetFeverTime();

        if(IsGameState(GameState.PLAYING))
        {
            SoundManager.Instance.PlayMusic("Infinity Music", false);

            cameraView.SetCameraPreset("Playing");
        }
    }

    void OnFeverTime()
    {
        isFeverTime = true;

        playerController.OnFever(true);

        playUI.OnFeverTime();

        spawnManager.FeverClearColumn(10, 3f);

        cameraView.SetCameraPreset("FeverTime");

        SoundManager.Instance.PlayMusic("Infinity Fever Music", false);
    }

    void ChargeFever()
    {
        // NOTE : 무한 모드에서만 피버 타임 적용
        if(gameType.IsEquals(GameType.INFINITY))
        {
            // NOTE : 피버 타임 중에는 충전되지 않음
            if(!isFeverTime)
            {
                feverCharge.SetValue(feverCharge.GetValue() + reChargeFever);

                if(feverCharge.GetValue() >= 2)
                {
                    feverTimer = Mathf.Clamp(feverTimer + regainFeverTime, 0, maxFever);
                    feverCharge.SetValue(0);

                    if(feverTimer >= maxFever)
                    {
                        OnFeverTime();
                    }
                }
            }
        }
    }

    void RewardScore(int reward = 1)
    {
        score.SetValue(score.GetValue() + reward);

        playUI.UpdateScore(score.GetValue());
    }

    void RegainHealth()
    {
        // NOTE : 무한 모드에서만 체력 회복
        if(gameType.IsEquals(GameType.INFINITY))
        {
            health = Mathf.Clamp(health + regainHealth, 0, currentGame.GetMaxHealth());

            // NOTE : 최대 점수 목표값 300,000점 (healthTime Min 12.5f / Max 30)
            ((InfinityLogic)currentGame).AddTimePerDamage();
        }
    }

    void CheckBranch()
    {
        Column column = spawnManager.GetNextColumn();
        if(column.HasBranch(playerController.GetDirectionType()))
        {
            if(isFeverTime)
            {
                spawnManager.SpawnEffect(column, 3);
            }
            else
            {
                spawnManager.SpawnEffect(column);

                GameOver();
            }
        }
    }

    public void OnPlayerMove()
    {
        if(IsGameState(GameState.PLAYING))
        {
            playUI.HideStartGuide();

            CheckBranch();

            spawnManager.NextColumn();

            // NOTE : 다음 기둥 가지에 걸리면 게임 오버라 진행되지 않음
            if(IsGameState(GameState.PLAYING))
            {
                RegainHealth();
                RewardScore();
                ChargeFever();

                if(!isFeverTime && gameType.IsEquals(GameType.INFINITY))
                {
                    disturbManager.UpdateTrigger(score.GetValue());
                }
                
                spawnManager.UpdateColumnMode(score.GetValue() >= PLAYABLE_HEIGHT_LIMIT);

                skyBoxManager.UpdateHeight(score.GetValue());

                if(score.GetValue() > PLAYABLE_HEIGHT_LIMIT)
                {
                    cameraView.SetFakeView();
                }
            }
        }
    }

    bool IsGameState(GameState targetState)
    {
        return gameState.IsEquals(targetState);
    }

    public void CloseChallenge()
    {
        gameState = GameState.LOBBY;

        canvasManager.SetPanel("Lobby");
    }

    public void CloseExtraMenu()
    {
        if(!canvasManager.ClosePanel())
        {
            canvasManager.SetPanel("Lobby");
        }

        gameState = GameState.LOBBY;

        SoundManager.Instance.PlayMusic("Infinity Music");
    }

    public void OpenChallegne()
    {
        if(IsGameState(GameState.LOBBY))
        {
            gameState = GameState.EXTRA_MENU;

            dailyChallengeManager.UpdateUI();

            canvasManager.SetPanel("Challenge");
        }
    }

    public void OpenSetting()
    {
        if(IsGameState(GameState.LOBBY))
        {
            gameState = GameState.EXTRA_MENU;

            canvasManager.SetPanel("Setting");
        }
    }

    public void OpenShop()
    {
        if(IsGameState(GameState.LOBBY))
        {
            gameState = GameState.EXTRA_MENU;

            canvasManager.SetPanel("Shop");
        }
    }

    public void OpenLeaderboard()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if(IsGameState(GameState.LOBBY))
        {
            gameState = GameState.EXTRA_MENU;

            playerController.SetControlLock(true);

            leaderboardManager.OnLeaderboard();

            canvasManager.SetPanel("Leaderboard");
        }
        #elif UNITY_ANDROID
        GPGS.ReportLeaderboard(gameType, bestScore.GetValue());
        GPGS.ShowLeaderboardUI();
        #endif
    }

    public void OpenCharacterSelect()
    {
        if(IsGameState(GameState.LOBBY))
        {
            // TODO : 추후에 플랫폼별로 호출 처리
            gameState = GameState.EXTRA_MENU;

            canvasManager.SetPanel("CharacterSelect");

            SoundManager.Instance.PlayMusic("Infinity Character");
        }
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(IsGameState(GameState.LOBBY))
            {
                GameStart();
            }
        }
    }

    public void OnLobbyEsc(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(IsGameState(GameState.LOBBY))
            {
                OpenSetting();
                return;
            }

            if(IsGameState(GameState.EXTRA_MENU))
            {
                CloseExtraMenu();
                return;
            }
        }
    }

    public void OnPlayingSpace(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(IsGameState(GameState.PAUSE))
            {
                ReturnLobby();
                return;
            }
        }
    }

    public void OnPlayingEsc(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(IsGameState(GameState.PLAYING))
            {
                GamePause();
                return;
            }
        }
    }

    public void OnResultSpace(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(resultUI.IsShowed())
            {
                GameRestart();
                return;
            }
        }
    }

    public void OnResultEsc(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(resultUI.IsShowed())
            {
                ReturnLobby();
                return;
            }
        }
    }
}
