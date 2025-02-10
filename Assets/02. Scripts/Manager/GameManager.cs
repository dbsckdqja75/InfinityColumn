using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static readonly int PLAYABLE_HEIGHT_LIMIT = 1000;

    MainGame currentGame;
    int currentModeIdx = 0;

    float lastPlayerHeight;

    [SerializeField] GameObject[] gameModeList;

    [Header("InGame UI")]
    [SerializeField] LobbyUI lobbyUI;
    [SerializeField] PlayUI playUI;
    [SerializeField] ResultUI resultUI;

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
        if(currentGame != null)
        {
            currentGame.UpdateLogic();

            if(currentGame.IsCurrentState(GameState.GAME_OVER))
            {
                UpdateFallHeight();
            }
        }
    }

    void Init()
    {
        cameraView.Init();
        spawnManager.Init();
        playerController.Init();
        playerController.AddMoveEvent(() => { OnPlayerMoved(); });

        ChangeGameMode(gameModeList[0]);

        ReturnLobby();
    }

    void GameReset()
    {
        currentGame.OnGameReset();

        playUI.ResetUI();

        playerController.ResetPosition();

        backgroundManager.ResetBackground();

        spawnManager.ResetPool();
        spawnManager.UpdateColumnMode(false);

        skyBoxManager.ResetColor();
        skyBoxManager.ResetHeight();

        disturbManager.ResetTrigger();
    }

    public void GameReady()
    {
        if(currentGame.IsCurrentState(GameState.LOBBY))
        {
            currentGame.OnGameReady();

            playerInput.SwitchCurrentActionMap("PlayerActions");

            canvasManager.SetPanel("Playing");

            SoundManager.Instance.PlaySound("Hit2");
        }
    }

    public void GameOver(bool isTimeOver = false)
    {
        currentGame.OnGameOver();

        lastPlayerHeight = playerController.GetPlayerHeight();
        playerController.SetControlLock(true);

        if(isTimeOver == true)
        {
            SoundManager.Instance.PlaySound("RefereeWhistle");
        }
        else
        {
            playerController.OnFall();
        }

        dailyChallengeManager.UpdateChallenge(currentGame.GetGameType(), currentGame.GetBestScore());

        playerInput.SwitchCurrentActionMap("ResultActions");

        canvasManager.SetPanel("GameOver");

        SoundManager.Instance.StopMusic();
    }

    public void GameRestart()
    {
        ReturnLobby();
        GameReady();
    }

    public void GamePause()
    {
        currentGame.OnGamePause();

        canvasManager.SetPanel("Pause");

        SoundManager.Instance.PlayMusic("Infinity Pause");
    }

    public void GameResume()
    {
        currentGame.OnGameResume();

        canvasManager.SetPanel("Playing");
    }

    public void ReturnLobby()
    {
        GameReset();

        cameraView.SetCameraPreset("Lobby");

        // NOTE : 카메라 위치 즉시 초기화 (Playing -> Lobby)
        if(!currentGame.IsCurrentState(GameState.PLAYING))
        {
            cameraView.RepositionView();
        }

        ResetUI();

        playerInput.SwitchCurrentActionMap("LobbyActions");

        canvasManager.SetPanel("Lobby");

        SoundManager.Instance.PlayMusic("Infinity Music");
    }

    void ChangeGameMode(GameObject gameMode)
    {
        if(currentGame != null)
        {
            Destroy(currentGame.gameObject);
        }

        currentGame = Instantiate(gameMode, this.transform).GetComponent<MainGame>();
        currentGame.Init(this);

        lobbyUI.UpdateGameModeText(currentGame.GetGameModeStringKey());

        ReturnLobby();
    }
    public void ToggleGameMode()
    {
        currentModeIdx = (int)Mathf.Repeat(currentModeIdx+1, gameModeList.Length);
        ChangeGameMode(gameModeList[currentModeIdx]);
    }

    void UpdateFallHeight()
    {
        float t = (playerController.GetPlayerHeight() / lastPlayerHeight);
        if(t <= 0.1f)
        {
            skyBoxManager.FixHeight(0);
            
            return;
        }

        int fallScore = (int)Mathf.Lerp(0, currentGame.GetScore(), t);

        skyBoxManager.UpdateHeight(fallScore);
    }

    public void UpdateBestScore()
    {
        currentGame.LoadGameRecord();
    }

    void ResetUI()
    {
        playUI.ResetUI();
        resultUI.ResetUI();

        CurrencyLabel.refresh?.Invoke();
    }

    void OnPlayerMoved()
    {
        currentGame.OnPlayerMoved();
    }

    public PlayUI GetPlayUI()
    {
        return playUI;
    }

    public ResultUI GetResultUI()
    {
        return resultUI;
    }

    public CameraView GetCameraView()
    {
        return cameraView;
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public SpawnManager GetSpawnManager()
    {
        return spawnManager;
    }

    public SkyBoxManager GetSkyBoxManager()
    {
        return skyBoxManager;
    }

    public DisturbManager GetDisturbManager()
    {
        return disturbManager;
    }

    #if UNITY_EDITOR || UNITY_STANDALONE
    public LeaderboardManager GetLeaderboardManager()
    {
        return leaderboardManager;
    }
    #endif

    #if UNITY_EDITOR || UNITY_ANDROID
    public GooglePlayManager GetGooglePlayManager()
    {
        return GPGS;
    }
    #endif

    public void CloseChallenge()
    {
        currentGame.OnExtraMenuClosed();

        canvasManager.SetPanel("Lobby");
    }

    public void CloseExtraMenu()
    {
        if(!canvasManager.ClosePanel())
        {
            canvasManager.SetPanel("Lobby");
        }

        currentGame.OnExtraMenuClosed();

        SoundManager.Instance.PlayMusic("Infinity Music");
    }

    public void OpenChallegne()
    {
        if(currentGame.OnExtraMenuOpen())
        {
            dailyChallengeManager.UpdateUI();

            canvasManager.SetPanel("Challenge");
        }
    }

    public void OpenSetting()
    {
        if(currentGame.OnExtraMenuOpen())
        {
            canvasManager.SetPanel("Setting");
        }
    }

    public void OpenShop()
    {
        if(currentGame.OnExtraMenuOpen())
        {
            canvasManager.SetPanel("Shop");
        }
    }

    public void OpenLeaderboard()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if(currentGame.OnExtraMenuOpen())
        {
            playerController.SetControlLock(true);

            leaderboardManager.OnLeaderboard();

            canvasManager.SetPanel("Leaderboard");
        }
        #elif UNITY_ANDROID
        GPGS.ReportLeaderboard(currentGame.GetGameType(), currentGame.GetBestScore());
        GPGS.ShowLeaderboardUI();
        #endif
    }

    public void OpenCharacterSelect()
    {
        if(currentGame.OnExtraMenuOpen())
        {
            canvasManager.SetPanel("CharacterSelect");

            SoundManager.Instance.PlayMusic("Infinity Character");
        }
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(currentGame.IsCurrentState(GameState.LOBBY))
            {
                GameReady();
            }
        }
    }

    public void OnLobbyEsc(InputAction.CallbackContext context)
    {
        if(context.phase.IsEquals(InputActionPhase.Performed))
        {
            if(currentGame.IsCurrentState(GameState.LOBBY))
            {
                OpenSetting();
                return;
            }

            if(currentGame.IsCurrentState(GameState.EXTRA_MENU))
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
            if(currentGame.IsCurrentState(GameState.PAUSE))
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
            if(currentGame.IsCurrentState(GameState.READY) || currentGame.IsCurrentState(GameState.PLAYING))
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
        if(currentGame is InfinityGame)
        {
            ((InfinityGame)currentGame).DebugFeverTime();
        }
    }

    public void DebugHealthMax()
    {
        if(currentGame is InfinityGame)
        {
            ((InfinityGame)currentGame).DebugInfinityHealth();
        }
    }
    #endif
}
