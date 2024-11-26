using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static readonly int HEIGHT_LIMIT = 1000;

    GameState gameState = GameState.LOADING;
    GameType gameType = GameType.INFINITY;
    GameLogic currentGame = new InfinityLogic();

    float health = 100f;
    float hpFillAmount = 1f;

    bool isFeverTime = false;
    float fever = 0f;
    SecureValue<int> feverCharge = new SecureValue<int>(0);
    float feverFillAmount = 0f;

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
    [SerializeField] int rewardUnit = 10;

    [Space(10)]
    [SerializeField] float healthfillSpeed = 10f;
    [SerializeField] float feverfillSpeed = 5f;

    [Space(10)]
    [SerializeField] float healthWarning = 0.3f;

    [Header("InGame UI")]
    [SerializeField] Image hpFillImage;
    [SerializeField] Image feverFillImage;
    [SerializeField] ImageColor hpFillColor;
    [SerializeField] ImageColor feverFillColor;
    [SerializeField] GameObject feverTimerObj;
    [SerializeField] TMP_Text hpTimerText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Animation scoreAnim;
    [SerializeField] LocalizeText gameModeText;

    [SerializeField] GameObject startTouchGuideObj;

    [Space(10)]
    [Header("GameOver UI")]
    [SerializeField] TMP_Text resultScoreText;
    [SerializeField] TMP_Text resultBestScoreText;
    [SerializeField] GameObject rewardBoxObj;
    [SerializeField] GameObject rewardVpObj;
    [SerializeField] TMP_Text rewardVpText;

    [Space(10)]
    [Header("Effect UI")]
    [SerializeField] GameObject warning_Effect;
    [SerializeField] GameObject fever_Effect;

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

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if(IsGameState(GameState.PLAYING))
        {
            UpdateUI();
            UpdateFeverTime();
            UpdateHealthTime();

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

        ReturnLobby();

        playerController.SetMoveLock(true);
        playerController.AddMoveEvent(() => { OnPlayerMove(); });

        LoadGameData();
    }

    void UpdateUI()
    {
        hpFillAmount = (health / currentGame.GetMaxHealth());
        hpFillImage.fillAmount = Mathf.Lerp(hpFillImage.fillAmount, hpFillAmount, healthfillSpeed * Time.deltaTime);

        feverFillAmount = (fever / maxFever);
        feverFillImage.fillAmount = Mathf.Lerp(feverFillImage.fillAmount, feverFillAmount, feverfillSpeed * Time.deltaTime);

        warning_Effect.SetActive(hpFillAmount <= healthWarning);

        if(warning_Effect.activeSelf)
        {
            hpFillColor.SetColorPreset(0);
        }
        else
        {
            hpFillColor.ResetColor();
        }
    }

    void UpdateFeverTime()
    {
        if(isFeverTime)
        {
            health = currentGame.GetMaxHealth();
            hpFillAmount = 1f;

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
            health = Mathf.Clamp(health, 0, currentGame.GetMaxHealth());
        }

        hpTimerText.text = ((int)health).ToString();
    }

    void UpdateBestScore()
    {
        bestScore.SetValue(PlayerPrefsManager.LoadData(currentGame.GetBestScoreDataKey(), 0));
    }

    void UpdateResultScore()
    {
        resultScoreText.text = String.Format("{0:N0}", score.GetValue().ToString());

        if(bestScore.GetValue() < score.GetValue())
        {
            // TODO : 갱신 표시 효과 추가 (신기록 달성)
            bestScore.SetValue(score.GetValue());

            SaveGameData();
        }

        resultBestScoreText.text = String.Format("{0:N0}", bestScore.GetValue().ToString());
    }

    void UpdateFall()
    {
        float t = (playerController.GetPlayerHeight() / lastPlayerHeight);

        int fallScore = (int)Mathf.Lerp(0, score.GetValue(), t);
        
        skyBoxManager.UpdateHeight(fallScore);
    }

    void ResetUI()
    {
        hpFillAmount = 1;
        feverFillAmount = 0;

        hpFillImage.fillAmount = hpFillAmount;
        feverFillImage.fillAmount = feverFillAmount;

        scoreText.text = score.GetValue().ToString();

        rewardBoxObj.SetActive(false);
        rewardVpObj.SetActive(false);

        CurrencyLabel.refresh?.Invoke();
    }

    public void UpdateGameModeText(string key)
    {
        gameModeText.SetLocaleString(key);
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

        currentGame.OnChangeGameMode(this);

        UpdateBestScore();

        ReturnLobby();
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

            if(IsGameState(GameState.PAUSE))
            {
                GameResume();
                return;
            }
        }
    }

    public void OnResultSpace(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(rewardBoxObj.activeSelf)
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
            if(rewardBoxObj.activeSelf)
            {
                ReturnLobby();
                return;
            }
        }
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

            rewardVpText.text = string.Format("+VP {0}", reward);
            rewardVpObj.SetActive(true);

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
        
        leaderboardManager.UpdateRecord(gameType);
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

    public void OpenLeaderboard()
    {
        if(IsGameState(GameState.LOBBY))
        {
            // TODO : 추후에 플랫폼별로 호출 처리
            gameState = GameState.EXTRA_MENU;

            playerController.SetMoveLock(true);

            leaderboardManager.OnLeaderboard();

            canvasManager.SetPanel("Leaderboard");
        }
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
        hpFillAmount = 1;

        ResetFeverTime();

        warning_Effect.SetActive(false);
        startTouchGuideObj.SetActive(true);

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
        feverFillAmount = 0f;

        playerController.Fever(false);

        fever_Effect.SetActive(false);
        feverFillColor.ResetColor();

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

        fever_Effect.SetActive(true);
        feverFillColor.SetColorPreset(0);

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
        scoreText.text = score.GetValue().ToString();

        scoreAnim.Stop();
        scoreAnim.Play();
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

    public void ExposeFeverUI()
    {
        feverTimerObj.SetActive(true);
    }

    public void ExposeHealthTimeUI()
    {
        hpTimerText.gameObject.SetActive(true);
    }

    void HideTouchGuide()
    {
        startTouchGuideObj.SetActive(false);
    }

    public void HideFeverUI()
    {
        feverTimerObj.SetActive(false);
    }

    public void HideHealthTimeUI()
    {
        hpTimerText.gameObject.SetActive(false);
    }

    public void OnPlayerMove()
    {
        if(!IsGameState(GameState.PLAYING))
            return;

        HideTouchGuide();

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
