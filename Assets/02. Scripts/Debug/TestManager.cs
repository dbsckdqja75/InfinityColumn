using UnityEngine;
using TMPro;

public class TestManager : MonoBehaviour
{
    [SerializeField] GameObject debugCanvas;

    #if UNITY_EDITOR
    [SerializeField] GameObject debugGroup;

    [SerializeField] GameManager gameManager;
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] DisturbManager disturbManager;
    [SerializeField] AutoPlayer autoPlayer;

    [SerializeField] TMP_Text autoPlayText, infinityHealthText, debugDisturbText, branchClearText;

    [Space(10)]
    [SerializeField] string captureFileDir;

    [SerializeField] DisturbData testDisturbData;

    bool isAutoPlay, isInfinityHealth, isDebugDisturb, isBranchClear;

    DisturbData originalDisturbData;

    void Awake()
    {
        UpdateUI();
    }

    void Update()
    {
        if(isInfinityHealth)
        {
            gameManager.DebugHealthMax();
        }

        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            Capture();
        }
    }

    void Start() 
    {
        debugCanvas.SetActive(true);
        debugGroup.SetActive(false);

        Application.targetFrameRate = 60;
    }

    void UpdateUI()
    {
        autoPlayText.text = string.Format("자동 플레이 {0}", isAutoPlay ? "ON" : "OFF");
        debugDisturbText.text = string.Format("방해 이벤트 디버그 {0}", isDebugDisturb ? "ON" : "OFF");
        branchClearText.text = string.Format("기둥 가지 클리어 {0}", isBranchClear ? "ON" : "OFF");
        infinityHealthText.text = string.Format("무한 체력 {0}", isInfinityHealth ? "ON" : "OFF");
    }

    void Capture()
    {
        string date = System.DateTime.UtcNow.Ticks.ToString();
        date = date.Replace("/","-");
        date = date.Replace(" ","_");

        ScreenCapture.CaptureScreenshot(captureFileDir + date + ".png");

        Debug.LogFormat("[Capture] {0}", captureFileDir + date + ".png");
    }

    public void ToggleAutoPlay()
    {
        isAutoPlay = !isAutoPlay;

        autoPlayer.SetAutoPlay(isAutoPlay);

        UpdateUI();
    }

    public void ToggleDebugDisturb()
    {
        isDebugDisturb = !isDebugDisturb;
        if(isDebugDisturb)
        {
            originalDisturbData = disturbManager.DebugGetDisturbData();
            disturbManager.DebugSetDisturbData(testDisturbData);
        }
        else
        {
            disturbManager.DebugSetDisturbData(originalDisturbData);
            originalDisturbData = null;
        }

        disturbManager.DebugSetToggle(isDebugDisturb);

        UpdateUI();
    }

    public void ForceDisturb()
    {
        disturbManager.DebugForceTrigger();
    }

    public void ForceFeverTime()
    {
        gameManager.DebugForceFeverTime();
    }
    
    public void ForceRewardVP()
    {
        CurrencyManager.Instance.RewardCurrency(CurrencyType.VOXEL_POINT, 10);
    }

    public void ToggleBranchClear()
    {
        isBranchClear = !isBranchClear;

        spawnManager.DebugBranchClear(isBranchClear);

        UpdateUI();
    }

    public void ToggleInfinityHealth()
    {
        isInfinityHealth = !isInfinityHealth;

        UpdateUI();
    }


    public void ToggleDebugMenu()
    {
        debugGroup.SetActive(!debugGroup.activeSelf);
    }
    #else
    void Awake() 
    {
        Destroy(debugCanvas);
        Destroy(this.gameObject);
    }
    #endif
}
