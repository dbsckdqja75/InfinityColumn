using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static readonly int HEIGHT_LIMIT = 1000;

    GameState gameState = GameState.LOADING;
    GameType gameType = GameType.INFINITY;
    GameLogic currentGame = new InfinityLogic();

    float health = 100f;

    bool isFeverTime = false;
    float fever = 0f;
    SecureValue<int> feverCharge = new SecureValue<int>(0);

    SecureValue<int> score = new SecureValue<int>(0);
    SecureValue<int> bestScore = new SecureValue<int>(0);

    float lastPlayerHeight;

    #if UNITY_EDITOR
    bool debug_infinityHealth = false;

    int debug_rewardScore = 1;
    #endif

    [Header("InGame Config")]
    [SerializeField] int maxFever = 100;
    [SerializeField] float feverTime = 20f;

    [Space(10)]
    [SerializeField] int rewardUnit = 100;

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
            UpdateFeverTime();
            UpdateHealthTime();
            UpdateUI();

            #if UNITY_EDITOR
            if(debug_infinityHealth)
            {
                health = 100f;
            }
            #endif
        }

        if(IsGameState(GameState.GAME_OVER))
        {
            UpdateFall();
        }
    }

    void Init()
    {
        cameraView.Init();
        spawnManager.Init();
        playerController.Init();

        #if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR && !UNITY_STANDALONE)
        if(Touchscreen.current != null)
        {
            playerInput.SwitchCurrentControlScheme("Mobile", Touchscreen.current);
        }

        playerInput.user.actions.Enable();
        playerInput.user.ActivateControlScheme("Mobile");
        #endif

        ReturnLobby();

        playerController.SetMoveLock(true);
        playerController.AddMoveEvent(() => { OnPlayerMove(); });

        LoadGameData();
    }

    void UpdateUI()
    {
        playUI.UpdateHealth(health / currentGame.GetMaxHealth());
        playUI.UpdateFever(fever / maxFever);
    }

    void UpdateFeverTime()
    {
        if(isFeverTime)
        {
            health = currentGame.GetMaxHealth();

            if(fever > 0)
            {
                fever -= (feverTime * Time.deltaTime);
                fever = Mathf.Clamp(fever, 0, maxFever);
            }
            else
            {
                ResetFeverTime();
            }
        }
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

    void UpdateFall()
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

            playerController.SetMoveLock(false);

            canvasManager.SetPanel("Playing");
        }
    }

    void GameOver()
    {
        gameState = GameState.GAME_OVER;

        cameraView.SetCameraPreset("GameOver");

        lastPlayerHeight = playerController.GetPlayerHeight();
        playerController.SetMoveLock(true);
        playerController.Fall();

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
        if(score >= rewardUnit)
        {
            int reward = (score / rewardUnit);

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

            playerController.SetMoveLock(true);
        
            canvasManager.SetPanel("Pause");

            SoundManager.Instance.PlayMusic("Infinity Pause");
        }
    }

    public void GameResume()
    {
        if(IsGameState(GameState.PAUSE))
        {
            gameState = GameState.PLAYING;

            playerController.SetMoveLock(false);

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

    public void CloseChallenge()
    {
        gameState = GameState.LOBBY;

        canvasManager.SetPanel("Lobby");
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

    public void CloseExtraMenu()
    {
        if(!canvasManager.ClosePanel())
        {
            canvasManager.SetPanel("Lobby");
        }

        gameState = GameState.LOBBY;

        SoundManager.Instance.PlayMusic("Infinity Music");
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

            playerController.SetMoveLock(true);

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

        fever = 0;
        feverCharge.SetValue(0);

        playerController.Fever(false);

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

        playerController.Fever(true);

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
                feverCharge.SetValue(feverCharge.GetValue() + 1);

                if(feverCharge.GetValue() >= 2)
                {
                    fever = Mathf.Clamp(fever + 1, 0, maxFever);
                    feverCharge.SetValue(0);

                    if(fever >= maxFever)
                    {
                        OnFeverTime();
                    }
                }
            }
        }
    }

    void RewardScore()
    {
        #if UNITY_EDITOR
        if(score.GetValue() > 1000)
        {
            score.SetValue(score.GetValue() + debug_rewardScore);
        }
        else
        {
            score.SetValue(score.GetValue() + 1);
        }
        #else
        score.SetValue(score.GetValue() + 1);
        #endif

        playUI.UpdateScore(score.GetValue());
    }

    void RewardHealth()
    {
        // NOTE : 무한 모드에서만 체력 회복
        if(gameType.IsEquals(GameType.INFINITY))
        {
            health = Mathf.Clamp(health + 4, 0, currentGame.GetMaxHealth());

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
                spawnManager.SpawnEffect(column, 3f);
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
        if(!IsGameState(GameState.PLAYING))
            return;

        playUI.HideStartGuide();

        CheckBranch();

        spawnManager.NextColumn();

        if(IsGameState(GameState.PLAYING))
        {
            RewardHealth();
            RewardScore();
            ChargeFever();

            if(!isFeverTime && gameType.IsEquals(GameType.INFINITY))
            {
                disturbManager.UpdateTrigger(score.GetValue());
            }
            
            spawnManager.UpdateColumnMode(score.GetValue() >= HEIGHT_LIMIT);

            skyBoxManager.UpdateHeight(score.GetValue());

            if(score.GetValue() > HEIGHT_LIMIT)
            {
                cameraView.SetFakeView();
            }
        }
    }

    bool IsGameState(GameState targetState)
    {
        return gameState.IsEquals(targetState);
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

    #if UNITY_EDITOR
    public void DebugForceFeverTime()
    {
        if(IsGameState(GameState.PLAYING))
        {
            fever = maxFever;
            feverCharge.SetValue(0);

            OnFeverTime();
        }
    }
    
    public void DebugSetInfinityHealth(bool isOn)
    {
        debug_infinityHealth = isOn;
    }

    public void DebugSetRewardScore(int reward)
    {
        debug_rewardScore = reward;
    }
    #endif
}
