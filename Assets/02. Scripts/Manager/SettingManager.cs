using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SettingManager : MonoBehaviour
{
    [SerializeField] CameraView cameraView;
    [SerializeField] BackgroundManager backgroundManager;
    [SerializeField] GameObject extraGlobalVolume;

    [Space(10)]
    [SerializeField] GameObject terrain;
    [SerializeField] GameObject groundPlane, groundQuad;
    [SerializeField] GameObject cloud;

    [Space(10)]
    [SerializeField] LocalizeText graphicsQualityText;
    [SerializeField] GameObject optimizedTextObj;
    [SerializeField] TMP_Text frameLimitText;
    [SerializeField] TMP_Text cameraProjectionText;
    [SerializeField] ToggleButton musicToggleButton;
    [SerializeField] ToggleButton sfxToggleButton;
    [SerializeField] GameObject restorePurchaseBox;
    [SerializeField] GameObject privacyBox;
    [SerializeField] GameObject contactBox;
    [SerializeField] GameObject discordBox;

    [Space(10)]
    [SerializeField] GameObject termsPanelObj;

    #if UNITY_EDITOR || UNITY_STANDALONE
    const int defaultGraphicsQuality = 2;
    #else
    const int defaultGraphicsQuality = 1;
    #endif

    void Awake()
    {
        Init();
    }

    void Init()
    {
        LoadSettingData();

        #if UNITY_ANDROID || UNITY_IPHONE
        LoadTermsStatus();
        #endif

        #if UNITY_STANDALONE || UNITY_EDITOR
        restorePurchaseBox.SetActive(false);
        privacyBox.SetActive(false);
        contactBox.SetActive(false);
        discordBox.SetActive(true);
        #endif

        // TODO : 출시 이후에 준비되면 삭제
        restorePurchaseBox.SetActive(false);
        discordBox.SetActive(false);
    }

    void LoadTermsStatus()
    {
        bool isAcceptTerms = PlayerPrefsManager.LoadData("AcceptTerms", false, false);

        if(isAcceptTerms == false)
        {
            termsPanelObj.SetActive(true);
        }
    }

    public void AcceptTheTerms()
    {
        PlayerPrefsManager.SaveData("AcceptTerms", true, false);

        termsPanelObj.SetActive(false);
    }

    public void LoadSettingData()
    {
        int graphicsQuality = PlayerPrefsManager.LoadData("GraphicsQualitySetting", defaultGraphicsQuality);
        UpdateGraphicsQuality(graphicsQuality);

        #if UNITY_ANDROID || UNITY_IPHONE
        ForceOptimizeGraphics();
        #endif

        int frameLimit = PlayerPrefsManager.LoadData("FrameLimitSetting", 60);
        UpdateFrameLimit(frameLimit);

        int cameraProjection = PlayerPrefsManager.LoadData("CameraProjectionSetting", 0);
        UpdateCameraProjection(cameraProjection == 0);

        int toggleMusic = PlayerPrefsManager.LoadData("ToggleMusicSetting", 1);
        UpdateToggleMusic(toggleMusic == 1);

        int toggleSFX = PlayerPrefsManager.LoadData("ToggleSfxSetting", 1);
        UpdateToggleSFX(toggleSFX == 1);

        int language = PlayerPrefsManager.LoadData("LanguageSetting", LocalizationManager.Instance.GetDefaultLanguage());
        UpdateLanguge(language);
    }

    public void SwitchGraphicsQuality()
    {
        int graphicsQuality = PlayerPrefsManager.LoadData("GraphicsQualitySetting", defaultGraphicsQuality);
        graphicsQuality = (int)Mathf.Repeat(graphicsQuality+1, 4);

        PlayerPrefsManager.SaveData("GraphicsQualitySetting", graphicsQuality);

        UpdateGraphicsQuality(graphicsQuality);
    }

    public void UpdateGraphicsQuality(int graphicsQuality)
    {
        QualitySettings.SetQualityLevel(graphicsQuality);
        extraGlobalVolume.SetActive(graphicsQuality >= 2); // NOTE : '중' 옵션부터 포스트 프로세싱 활성화

        backgroundManager.UpdatePresetQuality();

        switch(graphicsQuality)
        {
        case 0:
            graphicsQualityText.SetLocaleString("GraphicsQuality_VeryLow");
            break;
        case 1:
            graphicsQualityText.SetLocaleString("GraphicsQuality_Low");
            break;
        case 2:
            graphicsQualityText.SetLocaleString("GraphicsQuality_Medium");
            break;
        case 3:
            graphicsQualityText.SetLocaleString("GraphicsQuality_High");
            break;
        default:
            break;
        }
    }

    void ForceOptimizeGraphics()
    {
        if(SystemInfo.graphicsMemorySize <= 1024 || SystemInfo.systemMemorySize <= 2048)
        {
            cloud.SetActive(false);

            backgroundManager.EnableOptimization();

            optimizedTextObj.SetActive(true);
        }
    }

    public void ToggleFrameLimit()
    {
        int frameLimit = PlayerPrefsManager.LoadData("FrameLimitSetting", 60);
        frameLimit = (frameLimit == 60) ? 30 : 60;
        PlayerPrefsManager.SaveData("FrameLimitSetting", frameLimit);
        UpdateFrameLimit(frameLimit);
    }

    public void UpdateFrameLimit(int frameLimit)
    {
        cameraView.SetTargetFrameRate(frameLimit);
        frameLimitText.text = frameLimit.ToString();
    }

    public void ToggleCameraProjection()
    {
        int cameraProjection = PlayerPrefsManager.LoadData("CameraProjectionSetting", 0);
        cameraProjection = (cameraProjection == 0) ? 1 : 0;
        PlayerPrefsManager.SaveData("CameraProjectionSetting", cameraProjection);
        UpdateCameraProjection(cameraProjection == 0);
    }

    public void UpdateCameraProjection(bool isPerspective)
    {
        cameraView.SetCameraProjection(isPerspective);

        terrain.SetActive(isPerspective);
        groundPlane.SetActive(isPerspective);
        groundQuad.SetActive(!isPerspective);

        cameraProjectionText.text = isPerspective ? "3D" : "2D";
    }

    public void ToggleMusic()
    {
        int toggleMusic = PlayerPrefsManager.LoadData("ToggleMusicSetting", 1);
        toggleMusic = (toggleMusic == 0) ? 1 : 0;
        PlayerPrefsManager.SaveData("ToggleMusicSetting", toggleMusic);
        UpdateToggleMusic(toggleMusic == 1);
    }

    public void UpdateToggleMusic(bool toggleMusic)
    {
        if(toggleMusic)
        {
            SoundManager.Instance.MusicUnmute();
        }
        else
        {
            SoundManager.Instance.MusicMute();
        }

        musicToggleButton.SetToggle(toggleMusic);
    }

    public void ToggleSFX()
    {
        int toggleSFX = PlayerPrefsManager.LoadData("ToggleSfxSetting", 1);
        toggleSFX = (toggleSFX == 0) ? 1 : 0;
        PlayerPrefsManager.SaveData("ToggleSfxSetting", toggleSFX);
        UpdateToggleSFX(toggleSFX == 1);
    }

    public void UpdateToggleSFX(bool toggleSFX)
    {
        if(toggleSFX)
        {
            SoundManager.Instance.SfxUnmute();
        }
        else
        {
            SoundManager.Instance.SfxMute();
        }

        sfxToggleButton.SetToggle(toggleSFX);
    }

    public void SwitchLanguage()
    {
        int language = PlayerPrefsManager.LoadData("LanguageSetting", LocalizationManager.Instance.GetDefaultLanguage());
        language = LocalizationManager.Instance.SwitchLanguage(language);
        PlayerPrefsManager.SaveData("LanguageSetting", language);
    }

    public void UpdateLanguge(int language)
    {
        LocalizationManager.Instance.ChangeLanguage(language);
    }

    public void RestorePurchase()
    {
        // TODO : 추후 스토어 API 작업 시 적용
    }
}
