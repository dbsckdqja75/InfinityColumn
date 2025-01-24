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

    [SerializeField] TMP_Text autoPlayText, rewardScoreText, infinityHealthText, debugDisturbText, branchClearText;

    [SerializeField] DisturbData testDisturbData;

    bool isAutoPlay, isDebugDisturb, isBranchClear;
    int rewardScore = 1;

    DisturbData originalDisturbData;

    void Awake()
    {
        UpdateUI();
    }

    void Start() 
    {
        debugCanvas.SetActive(true);
        debugGroup.SetActive(false);
    }

    void UpdateUI()
    {
        autoPlayText.text = string.Format("자동 플레이 {0}", isAutoPlay ? "ON" : "OFF");
        debugDisturbText.text = string.Format("방해 이벤트 디버그 {0}", isDebugDisturb ? "ON" : "OFF");
        branchClearText.text = string.Format("기둥 가지 클리어 {0}", isBranchClear ? "ON" : "OFF");
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
