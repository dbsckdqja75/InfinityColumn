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

    bool isAutoPlay, isInfinityHealth, isDebugDisturb, isBranchClear;
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
        rewardScoreText.text = string.Format("점수 증가 ({0})", rewardScore.ToString());
        infinityHealthText.text = string.Format("무한 체력 {0}", isInfinityHealth ? "ON" : "OFF");
        debugDisturbText.text = string.Format("방해 이벤트 디버그 {0}", isDebugDisturb ? "ON" : "OFF");
        branchClearText.text = string.Format("기둥 가지 클리어 {0}", isBranchClear ? "ON" : "OFF");
    }

    public void ToggleAutoPlay()
    {
        isAutoPlay = !isAutoPlay;

        autoPlayer.SetAutoPlay(isAutoPlay);

        UpdateUI();
    }

    public void ToggleRewardScore()
    {
        switch(rewardScore)
        {
            case 1:
            rewardScore = 10;
            break;
            case 10:
            rewardScore = 100;
            break;
            case 100:
            rewardScore = 1000;
            break;
            case 1000:
            rewardScore = 1;
            break;
        }

        // gameManager.DebugSetRewardScore(rewardScore);

        UpdateUI();
    }

    public void ToggleInfinityHealth()
    {
        isInfinityHealth = !isInfinityHealth;

        // gameManager.DebugSetInfinityHealth(isInfinityHealth);

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
        // gameManager.DebugForceFeverTime();
    }

    public void ToggleBranchClear()
    {
        isBranchClear = !isBranchClear;

        spawnManager.DebugSetBranchClear(isBranchClear);

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
